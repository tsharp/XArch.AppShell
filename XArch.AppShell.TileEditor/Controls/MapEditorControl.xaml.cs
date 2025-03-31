using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.UI;
using XArch.AppShell.TileEditor.Models;

namespace XArch.AppShell.TileEditor.Controls
{
    /// <summary>
    /// Tile editor control with zoom, pan, tile placement and hover highlight.
    /// </summary>
    public partial class MapEditorControl : EditorControl
    {
        private readonly TileBrushTool _tileBrushTool;
        private TileLayer? _activeLayer;
        private TileMap? _map;

        private readonly Rectangle _hoverHighlight;
        private Point? _panStart;

        private bool _isPainting = false;
        private MouseButton _paintButton;
        private HashSet<Point> _alreadyPainted = new(); // Prevent duplicates during drag
        private readonly Dictionary<int, Rectangle> _tileVisuals = new();

        public int TileSize { get; set; } = 32;

        public MapEditorControl(TileBrushTool tileBrushTool, IEventManager eventManager, string filePath) : base(eventManager, filePath)
        {
            InitializeComponent();
            _tileBrushTool = tileBrushTool;
            _hoverHighlight = new Rectangle
            {
                Width = TileSize,
                Height = TileSize,
                Stroke = Brushes.Yellow,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(40, 255, 255, 0)),
                Visibility = Visibility.Hidden,
                IsHitTestVisible = false
            };

            Loaded += OnLoaded;

            TileCanvas.MouseMove += TileCanvas_MouseMove;
            TileCanvas.MouseDown += TileCanvas_MouseDown;
            TileCanvas.MouseUp += TileCanvas_MouseUp;
            TileCanvas.MouseLeave += (_, __) => _hoverHighlight.Visibility = Visibility.Hidden;
            // TileCanvas.MouseLeftButtonDown += TileCanvas_MouseButtonDown;
            // TileCanvas.MouseRightButtonDown += TileCanvas_MouseButtonDown;
            TileScrollViewer.PreviewMouseWheel += TileScrollViewer_MouseWheel;

            TileCanvas.Cursor = Cursors.Cross;
        }

        protected override void Save()
        {
            TileMapLoader.Save(_map, FilePath);

            string pngName = $"{System.IO.Path.GetFileNameWithoutExtension(FilePath)}.png";
            string directory = System.IO.Path.GetDirectoryName(FilePath);
            string pngFullPath = System.IO.Path.Combine(directory, pngName);

            ExportCanvasToPng(TileCanvas, pngFullPath);
        }

        public void ExportCanvasToPng(Canvas canvas, string outputPath, double dpi = 96)
        {
            // Ensure layout is fully measured and arranged
            canvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            canvas.Arrange(new Rect(canvas.DesiredSize));

            var width = _map.Width * TileSize; // (int)canvas.ActualWidth;
            var height = _map.Width * TileSize; // (int)canvas.ActualHeight;

            if (width == 0 || height == 0)
                return;

            var rtb = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
            rtb.Render(canvas);

            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            pngEncoder.Save(fs);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _map = TileMapLoader.Load(FilePath);
            SetActiveLayer("Terrain");
            RenderAllTiles();

            // Keep hover highlight on top
            GridOverlayCanvas.Children.Remove(_hoverHighlight);
            GridOverlayCanvas.Children.Add(_hoverHighlight);
        }

        public void SetTile(int x, int y, int typeId)
        {
            if (_activeLayer == null)
                return;

            _map.SetTile(_activeLayer, x, y, typeId);
        }

        //public void AddLayer(string name, int zIndex)
        //{
        //    var layer = new TileLayer { Name = name, ZIndex = zIndex };
        //    _layers.Add(layer);
        //    _layers.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));

        //    if (_activeLayer == null)
        //        _activeLayer = layer;
        //}

        public void SetActiveLayer(string name)
        {
            _activeLayer = _map.Layers.SingleOrDefault(l => l.Name == name);
        }

        private Rectangle CreateTileRect(int x, int y, int typeId)
        {
            var fill = Models.TileBrush.GetBrush(typeId).Fill;
            var rect = new Rectangle
            {
                Width = TileSize,
                Height = TileSize,
                Fill = fill,
                Stroke = Brushes.DarkGray,
                StrokeThickness = 0.35,
                Tag = null
            };

            Canvas.SetLeft(rect, x * TileSize);
            Canvas.SetTop(rect, y * TileSize);

            return rect;
        }

        private void UpdateTileVisual(int key, int x, int y, int typeId)
        {
            if (_tileVisuals.TryGetValue(key, out var rect))
            {
                var fill = Models.TileBrush.GetBrush(typeId).Fill;
                rect.Fill = fill;
            }
            else
            {
                rect = CreateTileRect(x, y, typeId);
                _tileVisuals[key] = rect;
                TileCanvas.Children.Add(rect);
            }

            // Ensure canvas redraws
            TileCanvas.InvalidateVisual();
        }

        private void RenderAllTiles()
        {
            if (_map == null) return;

            TileCanvas.Children.Clear();
            _tileVisuals.Clear();

            foreach (var layer in _map.Layers.OrderBy(l => l.ZIndex))
            {
                if (!layer.IsVisible) continue;

                foreach (var kvp in layer.Tiles)
                {
                    var key = kvp.Key;
                    var typeId = kvp.Value;
                    var (x, y) = _map.GetCoord(key);

                    var rect = CreateTileRect(x, y, typeId);
                    _tileVisuals[key] = rect;
                    TileCanvas.Children.Add(rect);
                }
            }
        }

        private Point GetWorldTileCoord(MouseEventArgs e)
        {
            var pos = e.GetPosition(TileCanvas);
            double scale = TileCanvasScale.ScaleX;

            double x = pos.X;
            double y = pos.Y;

            return new Point((int)(x / TileSize), (int)(y / TileSize));
        }


        private void TileCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPainting)
            {
                ApplyPaintAt(e);
                return;
            }

            if (_panStart.HasValue && e.MiddleButton == MouseButtonState.Pressed)
            {
                var current = e.GetPosition(TileScrollViewer);
                var delta = current - _panStart.Value;

                // Clamp
                TileCanvasOffset.X = Math.Min(TileCanvasOffset.X + delta.X, 0);
                TileCanvasOffset.Y = Math.Min(TileCanvasOffset.Y + delta.Y, 0);

                _panStart = current;
                return;
            }

            var tileCoord = GetWorldTileCoord(e);
            Canvas.SetLeft(_hoverHighlight, tileCoord.X * TileSize);
            Canvas.SetTop(_hoverHighlight, tileCoord.Y * TileSize);
            _hoverHighlight.Visibility = Visibility.Visible;
        }

        private void TileCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _panStart = e.GetPosition(TileScrollViewer);
                TileCanvas.Cursor = Cursors.ScrollAll;
                TileCanvas.CaptureMouse();
                return;
            }

            _isPainting = true;
            _paintButton = e.ChangedButton;
            _alreadyPainted.Clear();

            ApplyPaintAt(e);
        }

        private void ApplyPaintAt(MouseEventArgs e)
        {
            var tileCoord = GetWorldTileCoord(e);
            var tileKey = _map.GetKey((int)tileCoord.X, (int)tileCoord.Y);

            _eventManager.Publish("tileEditor.coordinate", $"{tileCoord.X},{tileCoord.Y}");

            // Out of bounds check (inclusive lower, exclusive upper)
            if (tileCoord.X < 0 || tileCoord.Y < 0 || tileCoord.X >= _map.Width || tileCoord.Y >= _map.Height)
                return;

            if (_alreadyPainted.Contains(tileCoord))
                return;

            _alreadyPainted.Add(tileCoord);

            if (_activeLayer == null)
                return;

            if (_paintButton == MouseButton.Left)
            {
                _activeLayer.Tiles[tileKey] = _tileBrushTool.SelectedBrush.TypeId;
                UpdateTileVisual(tileKey, (int)tileCoord.X, (int)tileCoord.Y, _tileBrushTool.SelectedBrush.TypeId);
            }
            else if (_paintButton == MouseButton.Right)
            {
                _activeLayer.Tiles[tileKey] = Models.TileBrush.Air.TypeId;
                UpdateTileVisual(tileKey, (int)tileCoord.X, (int)tileCoord.Y, Models.TileBrush.Air.TypeId);
            }

            IsDirty = true;
        }

        private void TileCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _panStart = null;
                TileCanvas.ReleaseMouseCapture();
                TileCanvas.Cursor = Cursors.Cross;
            }
            else if (_isPainting && e.ChangedButton == _paintButton)
            {
                _isPainting = false;
                _alreadyPainted.Clear();
            }
        }

        private void TileScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            const double zoomStep = 0.25;
            const double minZoom = 0.75;
            const double maxZoom = 2.0;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var position = e.GetPosition(TileCanvas);

                double zoomFactor = e.Delta > 0 ? (1 + zoomStep) : (1 - zoomStep);
                double newScale = Math.Clamp(TileCanvasScale.ScaleX * zoomFactor, minZoom, maxZoom);
                zoomFactor = newScale / TileCanvasScale.ScaleX;

                TileCanvasScale.ScaleX = newScale;
                TileCanvasScale.ScaleY = newScale;

                TileCanvasOffset.X = (TileCanvasOffset.X - position.X) * zoomFactor + position.X;
                TileCanvasOffset.Y = (TileCanvasOffset.Y - position.Y) * zoomFactor + position.Y;

                // Clamp
                TileCanvasOffset.X = Math.Min(TileCanvasOffset.X, 0);
                TileCanvasOffset.Y = Math.Min(TileCanvasOffset.Y, 0);


                e.Handled = true;
            }
        }
    }
}

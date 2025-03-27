using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using static System.Formats.Asn1.AsnWriter;

namespace XArch.AppShell.TileEditor
{
    /// <summary>
    /// Tile editor control with zoom, pan, tile placement and hover highlight.
    /// </summary>
    public partial class TileEditorControl : UserControl
    {
        private readonly List<TileLayer> _layers = new();
        private TileLayer? _activeLayer;

        private readonly Rectangle _hoverHighlight;
        private Point? _panStart;

        private bool _isPainting = false;
        private MouseButton _paintButton;
        private HashSet<Point> _alreadyPainted = new(); // Prevent duplicates during drag

        public int TileSize { get; set; } = 32;

        public TileEditorControl()
        {
            InitializeComponent();

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
            TileCanvas.MouseLeftButtonDown += TileCanvas_MouseButtonDown;
            TileCanvas.MouseRightButtonDown += TileCanvas_MouseButtonDown;
            TileScrollViewer.PreviewMouseWheel += TileScrollViewer_MouseWheel;
            TileScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;

            TileCanvas.Cursor = Cursors.Cross;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            GenerateTestGrid(20, 20);
            RenderTiles();
        }

        public void SetTile(int x, int y, Tile tile)
        {
            _activeLayer.Tiles[new Point(x, y)] = tile;
        }

        public void AddLayer(string name, int zIndex)
        {
            var layer = new TileLayer { Name = name, ZIndex = zIndex };
            _layers.Add(layer);
            _layers.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));

            if (_activeLayer == null)
                _activeLayer = layer;
        }

        public void SetActiveLayer(string name)
        {
            _activeLayer = _layers.FirstOrDefault(l => l.Name == name);
        }

        private void GenerateTestGrid(int width, int height)
        {
            _layers.Clear();
            _activeLayer = null;

            var layer = new TileLayer
            {
                Name = "Base Layer",
                ZIndex = 0
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    layer.Tiles[new Point(x, y)] = new Tile
                    {
                        X = x,
                        Y = y,
                        Type = "Grass",
                        Fill = (x + y) % 2 == 0 ? Brushes.DarkOliveGreen : Brushes.OliveDrab
                    };
                }
            }

            _layers.Add(layer);
            _activeLayer = _layers.First();
        }

        private void RenderTiles()
        {
            var viewport = new Rect(
                TileScrollViewer.HorizontalOffset,
                TileScrollViewer.VerticalOffset,
                TileScrollViewer.ViewportWidth,
                TileScrollViewer.ViewportHeight);

            RenderTiles(viewport);
        }

        private void RenderTiles(Rect viewport)
        {
            TileCanvas.Children.Clear();

            var scale = TileCanvasScale.ScaleX;
            var offsetX = TileCanvasOffset.X;
            var offsetY = TileCanvasOffset.Y;

            int startX = (int)Math.Floor((viewport.X - offsetX) / (TileSize * scale));
            int endX = (int)Math.Ceiling((viewport.X + viewport.Width - offsetX) / (TileSize * scale));
            int startY = (int)Math.Floor((viewport.Y - offsetY) / (TileSize * scale));
            int endY = (int)Math.Ceiling((viewport.Y + viewport.Height - offsetY) / (TileSize * scale));

            foreach (var layer in _layers.OrderBy(l => l.ZIndex))
            {
                if (!layer.IsVisible) continue;

                foreach (var kvp in layer.Tiles)
                {
                    var tile = kvp.Value;
                    var rect = new Rectangle
                    {
                        Width = TileSize,
                        Height = TileSize,
                        Fill = tile.Fill,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 0.5,
                        Tag = tile
                    };

                    Canvas.SetLeft(rect, tile.X * TileSize);
                    Canvas.SetTop(rect, tile.Y * TileSize);
                    TileCanvas.Children.Add(rect);
                }
            }

            TileCanvas.Children.Add(_hoverHighlight);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var viewport = new Rect(e.HorizontalOffset, e.VerticalOffset, e.ViewportWidth, e.ViewportHeight);
            RenderTiles(viewport);
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

                //TileCanvasOffset.X += delta.X;
                //TileCanvasOffset.Y += delta.Y;

                // Clamp
                TileCanvasOffset.X = Math.Min(TileCanvasOffset.X + delta.X, 0);
                TileCanvasOffset.Y = Math.Min(TileCanvasOffset.Y + delta.Y, 0);


                _panStart = current;
                RenderTiles();
                return;
            }

            var tileCoord = GetWorldTileCoord(e);
            Canvas.SetLeft(_hoverHighlight, tileCoord.X * TileSize);
            Canvas.SetTop(_hoverHighlight, tileCoord.Y * TileSize);
            _hoverHighlight.Visibility = Visibility.Visible;

            var pos = e.GetPosition(TileCanvas);
            Debug.WriteLine($"Tile Coord: {tileCoord.X}, {tileCoord.Y}, @ canvas (Offset: {TileCanvasOffset.X}, {TileCanvasOffset.Y}) (Pos: {pos.X},{pos.Y})");
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
            var key = new Point(tileCoord.X, tileCoord.Y);

            if (_alreadyPainted.Contains(key))
                return;

            _alreadyPainted.Add(key);

            if (_paintButton == MouseButton.Left)
            {
                if (_activeLayer != null && !_activeLayer.Tiles.ContainsKey(key))
                {
                    _activeLayer.Tiles[key] = new Tile
                    {
                        X = (int)tileCoord.X,
                        Y = (int)tileCoord.Y,
                        Type = "Placed",
                        Fill = Brushes.SteelBlue
                    };
                }
            }
            else if (_paintButton == MouseButton.Right)
            {
                _activeLayer?.Tiles.Remove(key);
            }

            RenderTiles();
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


        private void TileCanvas_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tileCoord = GetWorldTileCoord(e);
            var key = new Point(tileCoord.X, tileCoord.Y);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (!_activeLayer.Tiles.ContainsKey(key))
                {
                    _activeLayer.Tiles[key] = new Tile
                    {
                        X = (int)tileCoord.X,
                        Y = (int)tileCoord.Y,
                        Type = "Placed",
                        Fill = Brushes.SteelBlue
                    };
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                _activeLayer.Tiles.Remove(key);
            }

            RenderTiles();
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
                RenderTiles();
            }
        }
    }
}

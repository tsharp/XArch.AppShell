using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.TileEditor.Controls
{
    /// <summary>
    /// Interaction logic for TileBrushTool.xaml
    /// </summary>
    public partial class TileBrushTool : UserControl
    {
        public ObservableCollection<Models.TileBrush> TileBrushes { get; } = new();
        private readonly IEventManager _eventManager;
        public Models.TileBrush SelectedBrush { get; protected set; } = Models.TileBrush.Air;

        public TileBrushTool(IEventManager eventManager)
        {
            InitializeComponent();

            this._eventManager = eventManager;

            DataContext = this;

            foreach (var brush in Models.TileBrush.GetDefaultBrushes())
            {
                TileBrushes.Add(brush);
            }
        }

        private void Brush_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Models.TileBrush brush)
            {
                SelectedBrush = brush;
            }
        }
    }
}

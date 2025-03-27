using System.Windows.Media;

namespace XArch.AppShell.TileEditor
{
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; } = "Empty";
        public Brush Fill { get; set; } = Brushes.Gray;
    }
}

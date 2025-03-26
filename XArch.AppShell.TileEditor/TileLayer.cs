using System.Collections.Generic;
using System.Windows;

namespace XArch.AppShell.TileEditor
{
    public class TileLayer
    {
        public string Name { get; set; } = "Layer";
        public bool IsVisible { get; set; } = true;
        public int ZIndex { get; set; } = 0;

        public Dictionary<Point, Tile> Tiles { get; } = new();
    }

}

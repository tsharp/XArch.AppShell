using System.Collections.Generic;

namespace XArch.AppShell.TileEditor.Models
{
    public class TileLayer
    {
        public string Name { get; set; } = "Layer";
        public bool IsVisible { get; set; } = true;
        public int ZIndex { get; set; } = 0;

        // Efficient storage: key = y * width + x, value = tile type ID
        public Dictionary<int, int> Tiles { get; set; } = new();
    }

}

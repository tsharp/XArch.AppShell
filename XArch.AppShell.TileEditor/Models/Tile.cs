using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace XArch.AppShell.TileEditor.Models
{
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; } = "Empty";
        [JsonIgnore]
        public Brush Fill { get; set; } = Brushes.Gray;
        public HashSet<IEntity> Objects { get; set; } = new();
    }
}

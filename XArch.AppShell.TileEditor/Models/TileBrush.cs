using System.Linq;
using System.Windows.Media;

namespace XArch.AppShell.TileEditor.Models
{
    public class TileBrush
    {
        public string Name { get; set; } = "";
        public Brush Fill { get; set; } = Brushes.Transparent;
        public int TypeId { get; set; }

        public static TileBrush[] GetDefaultBrushes()
        {
            return new TileBrush[]
            {
                Air,
                Water,
                Grass,
                Dirt,
                Stone,
                Sand,
                Lava
            };
        }

        public static TileBrush? GetBrush(int typeId)
        {
            return GetDefaultBrushes().Where(b => b.TypeId == typeId).SingleOrDefault();
        }

        public static readonly TileBrush Air = new TileBrush
        {
            Name = "Air",
            Fill = Brushes.Transparent,
            TypeId = 0
        };

        public static readonly TileBrush Water = new TileBrush
        {
            Name = "Water",
            Fill = Brushes.Blue,
            TypeId = 1
        };

        public static readonly TileBrush Grass = new TileBrush
        {
            Name = "Grass",
            Fill = Brushes.Green,
            TypeId = 2
        };

        public static readonly TileBrush Dirt = new TileBrush
        {
            Name = "Dirt",
            Fill = new SolidColorBrush(Color.FromArgb(255, 116, 102, 59)),
            TypeId = 3
        };

        public static readonly TileBrush Stone = new TileBrush
        {
            Name = "Stone",
            Fill = Brushes.Gray,
            TypeId = 4
        };

        public static readonly TileBrush Sand = new TileBrush
        {
            Name = "Sand",
            Fill = Brushes.SandyBrown,
            TypeId = 6
        };
        public static readonly TileBrush Lava = new TileBrush
        {
            Name = "Lava",
            Fill = Brushes.Red,
            TypeId = 7
        };
    }
}

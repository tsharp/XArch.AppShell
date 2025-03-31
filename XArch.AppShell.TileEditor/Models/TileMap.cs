using System.Collections.Generic;

namespace XArch.AppShell.TileEditor.Models
{
    public class TileMap
    {
        public string Name { get; set; }
        public int Width { get; set; } = 25;
        public int Height { get; set; } = 25;
        public HashSet<TileLayer> Layers { get; set; } = new();

        public int GetKey(int x, int y) => y * Width + x;
        public (int x, int y) GetCoord(int key) => (key % Width, key / Width);

        public void SetTile(TileLayer layer, int x, int y, int typeId)
        {
            layer.Tiles[GetKey(x, y)] = typeId;
        }

        public bool TryGetTile(TileLayer layer, int x, int y, out int typeId)
        {
            return layer.Tiles.TryGetValue(GetKey(x, y), out typeId);
        }

        public void RemoveTile(TileLayer layer, int x, int y)
        {
            layer.Tiles.Remove(GetKey(x, y));
        }
    }
}

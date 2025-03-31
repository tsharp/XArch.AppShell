using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XArch.AppShell.TileEditor.Models
{
    internal static class TileMapLoader
    {
        private readonly static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            IncludeFields = true,
            IgnoreReadOnlyProperties = true,
            IgnoreReadOnlyFields = true,
            AllowOutOfOrderMetadataProperties = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static TileMap Load(string fileName)
        {
            // File Does Not Exist
            if (File.Exists(fileName) == false)
                return NewTileMap();

            // Empty File
            if (new FileInfo(fileName).Length == 0)
                return NewTileMap();

            // Open File Stream
            using (var stream = File.OpenRead(fileName))
                return JsonSerializer.Deserialize<TileMap>(stream, _options);
        }

        public static void Save(TileMap tileMap, string fileName)
        {
            // Open File Stream
            using (var stream = File.Create(fileName))
            {
                JsonSerializer.Serialize(stream, tileMap, _options);
            }
        }

        private static TileMap NewTileMap()
        {
            var tileMap = new TileMap();

            tileMap.Layers.Add(new TileLayer
            {
                ZIndex = 0,
                Name = "Terrain", // Grass, dirt, water, base layer
            });

            tileMap.Layers.Add(new TileLayer
            {
                ZIndex = 1,
                Name = "Foliage", // Trees, bushes, plants
            });

            tileMap.Layers.Add(new TileLayer
            {
                ZIndex = 2,
                Name = "Structures", // Walls, buildings, bridges
            });

            tileMap.Layers.Add(new TileLayer
            {
                ZIndex = 3,
                Name = "Collision", // Walkable/blocking data
            });

            tileMap.Layers.Add(new TileLayer
            {
                ZIndex = 4,
                Name = "Lighting", // Light levels, fog, shadow overlays
            });

            tileMap.Layers.Add(new TileLayer
            {
                ZIndex = 5,
                Name = "Hidden", // GM-only layer for secret data
            });

            return tileMap;
        }
    }
}

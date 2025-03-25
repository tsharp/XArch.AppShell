namespace XArch.AppShell.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class AtlasProject
    {
        public string FilePath { get; set; }

        public string RootDirectory => Path.GetDirectoryName(FilePath);

        public string Name { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }

        public List<string> Maps { get; set; } = new();
        public List<string> Lore { get; set; } = new();

        public override string ToString()
        {
            return $"{Name} ({Maps.Count} maps, {Lore.Count} lore files)";
        }
    }
}

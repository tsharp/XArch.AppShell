using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace XArch.AppShell.Sdk
{
    public static class AtlasProjectService
    {
        private class RawProjectMetadata
        {
            public string Name { get; set; }
            public string Author { get; set; }
            public DateTime Created { get; set; }
        }

        private class RawProjectModel
        {
            public string Type { get; set; }
            public string Version { get; set; }
            public RawProjectMetadata Metadata { get; set; }
        }

        public static AtlasProject LoadFromDirectory(string directory)
        {
            var file = Directory.GetFiles(directory, "*.atlasproj").FirstOrDefault();
            if (file == null)
                throw new FileNotFoundException("No .atlasproj file found.");

            var raw = JsonSerializer.Deserialize<RawProjectModel>(File.ReadAllText(file), new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreReadOnlyFields = true,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (raw?.Type != "Kuiper.AtlasProject")
                throw new InvalidDataException("Unsupported project type.");

            if (raw.Version != "1.0")
                throw new InvalidDataException("Unsupported project format version.");

            var matcher = new Matcher();
            matcher.AddInclude("**/*.kmap");
            matcher.AddInclude("**/*.md");

            var result = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directory)));

            return new AtlasProject
            {
                FilePath = file,
                Name = raw.Metadata?.Name ?? "Unnamed",
                Author = raw.Metadata?.Author ?? "Unknown",
                Created = raw.Metadata?.Created ?? DateTime.UtcNow,
                Maps = result.Files.Where(f => f.Path.EndsWith(".kmap")).Select(f => f.Path).ToList(),
                Lore = result.Files.Where(f => f.Path.EndsWith(".md")).Select(f => f.Path).ToList()
            };
        }

        public static void Save(AtlasProject project)
        {
            var json = JsonSerializer.Serialize(new RawProjectModel
            {
                Type = "Kuiper.AtlasProject",
                Version = "1.0",
                Metadata = new RawProjectMetadata
                {
                    Name = project.Name,
                    Author = project.Author,
                    Created = project.Created.ToUniversalTime()
                }
            }, new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreReadOnlyFields = true,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            File.WriteAllText(project.FilePath, json);
        }
    }
}

using System.Collections.ObjectModel;
using System.IO;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    public class FileSystemItem : IFileSystemItem
    {
        public string Name { get; set; } = "";
        public string FullPath { get; set; } = "";
        public bool IsDirectory { get; set; } = false;
        public bool IsExpanded { get; set; }
        public ObservableCollection<FileSystemItem> Children { get; set; } = new();
        public string IconGlyph
        {
            get
            {
                if (IsDirectory) return "\uE8B7";
                return Path.GetExtension(FullPath)?.ToLower() switch
                {
                    ".cs" => "\uE7AD", // Code file
                    ".json" => "\uE60B", // Braces
                    _ => "\uE8A5", // Generic file
                };
            }
        }
    }
}

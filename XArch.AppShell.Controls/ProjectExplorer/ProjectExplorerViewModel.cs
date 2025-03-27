using System.Collections.ObjectModel;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    public class ProjectExplorerViewModel : IFileSystemItem
    {
        public ObservableCollection<FileSystemItem> Items { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public bool IsDirectory => true;

        public bool IsExpanded
        {
            get => true;
            set { }
        }
    }
}

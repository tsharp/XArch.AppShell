namespace XArch.AppShell.Controls.ProjectExplorer
{
    public interface IFileSystemItem
    {
        string Name { get; }
        string FullPath { get; }
        bool IsDirectory { get; }
        bool IsExpanded { get; set; }
    }
}

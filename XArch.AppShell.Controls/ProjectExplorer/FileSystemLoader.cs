using System.Collections.ObjectModel;
using System.IO;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    public class FileSystemLoader
    {
        public ObservableCollection<FileSystemItem> LoadTree(string rootPath)
        {
            var root = LoadNode(rootPath);
            return root?.Children ?? new ObservableCollection<FileSystemItem>();
        }

        private FileSystemItem? LoadNode(string path)
        {
            try
            {
                var isDirectory = Directory.Exists(path);
                var item = new FileSystemItem
                {
                    Name = Path.GetFileName(path),
                    FullPath = path,
                    IsDirectory = isDirectory
                };

                if (isDirectory)
                {
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        var child = LoadNode(dir);
                        if (child != null)
                            item.Children.Add(child);
                    }

                    foreach (var file in Directory.GetFiles(path))
                    {
                        if (Path.GetExtension(file) == ".atlasproj")
                            continue;

                        item.Children.Add(new FileSystemItem
                        {
                            Name = Path.GetFileName(file),
                            FullPath = file,
                            IsDirectory = false
                        });
                    }
                }

                return item;
            }
            catch
            {
                return null;
            }
        }
    }

}

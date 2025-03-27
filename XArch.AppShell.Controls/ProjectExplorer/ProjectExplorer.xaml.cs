using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    /// <summary>
    /// Interaction logic for ProjectExplorer.xaml
    /// </summary>
    public partial class ProjectExplorer : UserControl
    {
        private readonly IEventManager _eventManager;
        private readonly Dictionary<string, bool> _expandedPaths = new();
        private string _rootPath;
        private FileSystemWatcher? _watcher;

        public ObservableCollection<ProjectExplorerViewModel> Root { get; set; } = new();

        public ProjectExplorer(IEventManager eventManager)
        {
            this._eventManager = eventManager;
            this._rootPath = string.Empty;
            InitializeComponent();
            DataContext = this;
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is FileSystemItem item && !item.IsDirectory)
            {
                // Send to view manager (optional)
                Debug.WriteLine($"Selected file: {item.FullPath}");
            }
        }

        internal void LoadProject(string rootPath)
        {
            Root.Clear();

            var loader = new FileSystemLoader();
            var projectItem = loader.LoadTree(rootPath);
            var rootFolderInfo = new DirectoryInfo(rootPath);
            this._rootPath = rootPath;

            Root.Add(new ProjectExplorerViewModel
            {
                Name = $"Project '{rootFolderInfo.Name}'",
                FullPath = rootPath,
                Items = projectItem
            });

            AttachContextMenus(ExplorerTree.Items, ExplorerTree.ItemContainerGenerator);

            SetupWatcher(rootPath);
        }

        private void SetupWatcher(string path)
        {
            _watcher?.Dispose();

            _watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
            };

            _watcher.Created += (_, _) => RefreshTree();
            _watcher.Deleted += (_, _) => RefreshTree();
            _watcher.Renamed += (_, _) => RefreshTree();
            _watcher.Changed += (_, _) => RefreshTree();

            _watcher.EnableRaisingEvents = true;
        }

        private void ApplyExpansionState(FileSystemItem item)
        {
            if (item.IsDirectory && _expandedPaths.TryGetValue(item.FullPath, out var isExpanded))
            {
                item.IsExpanded = isExpanded;
            }

            foreach (var child in item.Children)
            {
                ApplyExpansionState(child);
            }
        }

        private void RefreshTree()
        {
            Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrWhiteSpace(_rootPath)) return;

                SaveExpandedState(Root.SelectMany(r => r.Items));

                var loader = new FileSystemLoader();
                var updatedItems = loader.LoadTree(_rootPath);

                Root[0].Items.Clear();
                foreach (var item in updatedItems)
                {
                    ApplyExpansionState(item);
                    Root[0].Items.Add(item);
                }

                AttachContextMenus(ExplorerTree.Items, ExplorerTree.ItemContainerGenerator);
            });
        }

        private void SaveExpandedState(IEnumerable<FileSystemItem> items)
        {
            foreach (var item in items)
            {
                if (item.IsDirectory)
                {
                    _expandedPaths[item.FullPath] = item.IsExpanded;
                    SaveExpandedState(item.Children);
                }
            }
        }

        private void AddNewFolder(IFileSystemItem item)
        {
            Dispatcher.Invoke(() =>
            {
                if (!Directory.Exists(item.FullPath)) return;

                var dialog = new InputDialog("Enter new folder name:");
                if (dialog.ShowDialog() == true)
                {
                    var folderName = dialog.ResponseText;
                    if (string.IsNullOrWhiteSpace(folderName)) return;

                    var newFolderPath = Path.Combine(item.FullPath, folderName);
                    Directory.CreateDirectory(newFolderPath);

                    // ✅ Mark this folder path to remain expanded
                    _expandedPaths[item.FullPath] = true;

                    _eventManager.Publish("project.folder.created", item.FullPath);
                }
            });
        }

        private void AddNewFile(IFileSystemItem item)
        {
            Dispatcher.Invoke(() =>
            {
                string directoryName = item.IsDirectory ? item.FullPath : Path.GetDirectoryName(item.FullPath);
                if (!Directory.Exists(directoryName)) return;

                var dialog = new InputDialog("Enter new file name:");
                if (dialog.ShowDialog() == true)
                {
                    var fileName = dialog.ResponseText;
                    if (string.IsNullOrWhiteSpace(fileName)) return;

                    var newFilePath = Path.Combine(directoryName, fileName);
                    File.WriteAllText(newFilePath, "");

                    // ✅ Mark the parent directory to stay expanded
                    _expandedPaths[directoryName] = true;

                    _eventManager.Publish("project.file.created", newFilePath);
                    _eventManager.Publish("project.file.open", newFilePath);
                }
            });
        }

        private void OpenFileItem(IFileSystemItem item)
        {
            Dispatcher.Invoke(() =>
            {
                _eventManager.Publish("project.file.open", item.FullPath);
            });
        }

        private void AttachContextMenus(ItemCollection items, ItemContainerGenerator generator)
        {
            foreach (var obj in items)
            {
                if (generator.ContainerFromItem(obj) is not TreeViewItem container)
                    continue;

                if (obj is IFileSystemItem folderItem && folderItem.IsDirectory)
                {
                    var contextMenu = new ContextMenu();

                    var newItem = new MenuItem { Header = "New Item..." };
                    var newFolder = new MenuItem { Header = "New Folder ..." };

                    newItem.Click += (_, _) => AddNewFile(folderItem);
                    newFolder.Click += (_, _) => AddNewFolder(folderItem);

                    contextMenu.Items.Add(newItem);
                    contextMenu.Items.Add(newFolder);

                    if (obj is not ProjectExplorerViewModel)
                    {
                        var deleteItem = new MenuItem { Header = "Delete ..." };
                        deleteItem.Click += (_, _) => DeleteItem(folderItem);
                        contextMenu.Items.Add(deleteItem);
                    }

                    container.ContextMenu = contextMenu;
                }
                else if (obj is IFileSystemItem fileItem)
                {
                    var contextMenu = new ContextMenu();
                    var deleteItem = new MenuItem { Header = "Delete ..." };
                    deleteItem.Click += (_, _) => DeleteItem(fileItem);
                    contextMenu.Items.Add(deleteItem);
                    container.ContextMenu = contextMenu;

                    container.MouseDoubleClick += (_, _) => OpenFileItem(fileItem);
                }

                if (container.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    AttachContextMenus(container.Items, container.ItemContainerGenerator);
                }
                else
                {
                    container.ItemContainerGenerator.StatusChanged += (s, e) =>
                    {
                        if (container.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                        {
                            AttachContextMenus(container.Items, container.ItemContainerGenerator);
                        }
                    };
                }
            }
        }

        private void DeleteItem(IFileSystemItem item)
        {
            Dispatcher.Invoke(() =>
            {
                if (item.IsDirectory)
                {
                    Directory.Delete(item.FullPath, true);
                    _eventManager.Publish("project.folder.deleted", item.FullPath);
                }
                else
                {
                    File.Delete(item.FullPath);
                    _eventManager.Publish("project.file.deleted", item.FullPath);
                }
            });
        }
    }
}

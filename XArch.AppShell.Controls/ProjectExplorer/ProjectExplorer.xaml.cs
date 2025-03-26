using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    /// <summary>
    /// Interaction logic for ProjectExplorer.xaml
    /// </summary>
    public partial class ProjectExplorer : UserControl
    {
        private readonly IServiceProvider _services;

        public ProjectExplorer(IServiceProvider services)
        {
            this._services = services;
            InitializeComponent();
        }

        public void LoadProject(string projectRoot)
        {
            var rootItem = new TreeViewItem
            {
                Header = Path.GetFileName(projectRoot),
                Tag = projectRoot,
                IsExpanded = true
            };

            LoadDirectoryRecursive(projectRoot, rootItem);
            ProjectTree.Items.Clear();
            ProjectTree.Items.Add(rootItem);
        }

        private void LoadDirectoryRecursive(string path, TreeViewItem parent)
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                var dirItem = new TreeViewItem
                {
                    Header = Path.GetFileName(dir),
                    Tag = dir,
                    IsExpanded = false
                };
                LoadDirectoryRecursive(dir, dirItem);
                parent.Items.Add(dirItem);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                var fileItem = new TreeViewItem
                {
                    Header = Path.GetFileName(file),
                    Tag = file
                };
                parent.Items.Add(fileItem);
            }
        }

        private void ProjectTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem item && File.Exists(item.Tag?.ToString()))
            {
                string filePath = item.Tag.ToString()!;
                // TODO: route to open file in editor
            }
        }

        private void ProjectTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Optional: context menu setup here
        }
    }
}

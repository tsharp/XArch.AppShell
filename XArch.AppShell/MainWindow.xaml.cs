using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell;
using XArch.AppShell.Framework.UI;
using XArch.AppShell.Sdk;

namespace XArch.AppShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AtlasStudioContext.Current.Initialize();
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            // Set default directory to user's Documents folder
            var dlg = new Microsoft.Win32.OpenFolderDialog()
            {
                InitialDirectory = "C:\\workspace_dnd", // Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false,
            };

            if (dlg.ShowDialog() == true)
            {
                // var project = new AtlasProject();
                // AtlasProjectService.Save(project, dlg.FileName);
                AtlasProjectService.Save(new AtlasProject()
                {
                    FilePath = $"{dlg.FolderName}\\project.atlasproj",
                    Name = "New Project",
                    Author = "Unknown",
                    Created = DateTime.UtcNow
                });

                AtlasStudioContext.Current.OpenProject(dlg.FolderName);
            }
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            // Set default directory to user's Documents folder
            var dlg = new Microsoft.Win32.OpenFolderDialog()
            {
                InitialDirectory = "C:\\workspace_dnd", // Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false,
            };

            if (dlg.ShowDialog() == true)
            {
                AtlasStudioContext.Current.OpenProject(dlg.FolderName);
            }

            var factory = AtlasStudioContext.Current.Services.GetRequiredService<IEditorFactory>();
            var view = factory.Create("test.md");
            var control = view.GetControl();

            AddEditorTab("test.md", control);

        }

        private void AddEditorTab(string title, UIElement control)
        {
            var tab = new TabItem
            {
                Header = title,
                Content = control
            };

            MainTabControl.Items.Add(tab);
            MainTabControl.SelectedItem = tab;
        }
    }
}
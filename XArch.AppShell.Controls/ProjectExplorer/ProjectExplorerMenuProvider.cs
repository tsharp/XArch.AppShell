using System.Collections.Generic;

using Microsoft.Win32;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.Menu;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    internal class ProjectExplorerMenuProvider : IMenuProvider
    {
        private readonly IEventManager eventManager;

        public ProjectExplorerMenuProvider(IEventManager eventManager)
        {
            this.eventManager = eventManager;
        }

        public IEnumerable<MenuEntry> GetMenus()
        {
            yield return new MenuEntry("_File", new[]
            {
                new MenuEntry("Open")
                {
                    Command = new RelayCommand<object>(e =>
                    {
                        OpenFolderDialog openFolderDialog = new OpenFolderDialog()
                        {
                            Title = "Open Project Folder",
                            Multiselect = false
                        };

                        if (openFolderDialog.ShowDialog().GetValueOrDefault())
                        {
                            eventManager.Publish("atlas.project.open", openFolderDialog.FolderName);
                        }
                    })
                }
            });
        }
    }
}

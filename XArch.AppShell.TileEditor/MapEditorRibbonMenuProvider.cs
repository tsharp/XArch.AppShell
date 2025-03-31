using System.Collections.Generic;

using Microsoft.Win32;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.Menu;
using XArch.AppShell.TileEditor.Controls;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    internal class MapEditorRibbonMenuProvider : IMenuProvider
    {
        private readonly IEventManager eventManager;

        public MapEditorRibbonMenuProvider(IEventManager eventManager)
        {
            this.eventManager = eventManager;
        }

        public string ContextMenuId => throw new System.NotImplementedException();

        public MenuType MenuType => MenuType.Toolbar;

        public IEnumerable<MenuEntry> GetMenuItems()
        {
            yield return new MenuEntry("Map", new[]
            {
                new MenuEntry("Layers")
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
                },
                new MenuEntry() { IsSeparator = true },
                new MenuEntry("Properties")
                {
                    Children = new List<MenuEntry>
                    {
                        new MenuEntry("Map Properties"),
                        new MenuEntry("Layer Properties")
                    }
                },
                new MenuEntry() 
                {
                    ControlType = typeof(MapEditorRibbonTool)
                }
            });
        }
    }
}

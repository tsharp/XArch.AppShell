using System;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Controls.ProjectExplorer;
using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.Menu;
using XArch.AppShell.Framework.UI;
using XArch.AppShell.TileEditor.Controls;

namespace XArch.AppShell.TileEditor
{
    [AtlasStudioPlugin("Markdown Editor Plugin", "Provides basic Markdown editing and preview.")]
    public class MapEditorPlugin : IAtlasStudioPlugin
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            IMenuManager menuManager = serviceProvider.GetRequiredService<IMenuManager>();
            IViewManager viewManager = serviceProvider.GetRequiredService<IViewManager>();
            IEventManager eventManager = serviceProvider.GetRequiredService<IEventManager>();
            IFileEditorManager editorManager = serviceProvider.GetRequiredService<IFileEditorManager>();
            editorManager.RegisterProvider(serviceProvider.GetRequiredService<MapEditorFactory>());
            menuManager.RegisterProvider(new MapEditorRibbonMenuProvider(eventManager));

            viewManager.RegisterTool(DockLocation.Left, "tools.tile.brushes", "Tile Brushes", serviceProvider.GetRequiredService<TileBrushTool>(),
                canHide: true,
                hiddenByDefault: false);
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<MapEditorFactory>();
            services.AddSingleton<TileBrushTool>();
            services.AddSingleton<MapEditorRibbonTool>();
        }
    }
}

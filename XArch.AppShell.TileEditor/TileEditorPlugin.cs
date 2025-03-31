using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Controls.ProjectExplorer;
using XArch.AppShell.Framework;
using XArch.AppShell.Framework.UI;
using XArch.AppShell.TileEditor.Controls;

namespace XArch.AppShell.TileEditor
{
    [AtlasStudioPlugin("Markdown Editor Plugin", "Provides basic Markdown editing and preview.")]
    public class TileEditorPlugin : IAtlasStudioPlugin
    {
        public void Configure(IAppContext appContext)
        {
            appContext.EditorManager.RegisterProvider(appContext.Services.GetRequiredService<TileEditorFactory>());
            appContext.MenuManager.RegisterProvider(new TileEditorRibbonMenuProvider(appContext.EventManager));

            appContext.ViewManager.RegisterTool(DockLocation.Left, "tools.tile.brushes", "Tile Brushes", appContext.Services.GetRequiredService<TileBrushTool>(),
                canHide: true,
                hiddenByDefault: false);
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<TileEditorFactory>();
            services.AddSingleton<TileBrushTool>();
            services.AddSingleton<MapEditorRibbonTool>();
        }
    }
}

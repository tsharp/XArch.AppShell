
using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.Menu;
using XArch.AppShell.Framework.UI;
using XArch.AppShell.Providers;

namespace XArch.AppShell.Core
{
    [AtlasStudioPlugin("Core", "Core System Plugin")]
    internal class CorePlugin : IAtlasStudioPlugin
    {
        public void Configure(IAppContext appContext)
        {
            MenuManager menuManager = appContext.Services.GetRequiredService<MenuManager>();
            menuManager.RegisterProvider(new CoreMainMenuProvider());
        }

        public void RegisterServices(IServiceCollection services)
        {
            // This registers services for both internal use and external use (outside of the core app)
            services.AddInternalSingleton<IMenuManager, MenuManager>();
            services.AddInternalSingleton<IViewManager, DockViewManager>();
            services.AddInternalSingleton<IEventManager, EventManager>();
            services.AddInternalSingleton<IFileEditorManager, FileEditorManager>();
        }
    }
}

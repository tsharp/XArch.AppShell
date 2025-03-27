
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
        public void Configure(IServiceProvider serviceProvider)
        {
            MenuManager menuManager = serviceProvider.GetRequiredService<MenuManager>();
            menuManager.RegisterProvider(new CoreMenuProvider());
        }

        public void RegisterServices(IServiceCollection services)
        {
            // This registers services for both internal use and external use (outside of the core app)
            services.AddInternalSingleton<IMenuManager, MenuManager>();
            services.AddInternalSingleton<IViewManager, DockViewManager>();
            services.AddInternalSingleton<IEventManager, EventManager>();
        }
    }
}

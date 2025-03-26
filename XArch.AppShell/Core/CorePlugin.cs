
using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
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
            services.AddSingleton<MenuManager>();
        }
    }
}

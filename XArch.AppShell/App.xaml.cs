using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using XArch.AppShell.Core;
using XArch.AppShell.Framework;

namespace XArch.AppShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Lazy<ServiceProvider> _services = new Lazy<ServiceProvider>(() => InitializeApplication());

        public static IServiceProvider Services => _services.Value;

        private static ServiceProvider InitializeApplication()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();
            IAtlasStudioPlugin[] plugins = PluginLoader.LoadPlugins(serviceDescriptors);
            ServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();
            
            // Handle the core plugin first
            plugins.OfType<CorePlugin>().Single().Configure(serviceProvider);

            foreach (IAtlasStudioPlugin plugin in plugins)
            {
                if (plugin is CorePlugin corePlugin)
                {
                    continue;
                }

                plugin.Configure(serviceProvider);
            }

            return serviceProvider;
        }

        public App()
        {
            this.Exit += App_OnExit;
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            _services.Value.Dispose();
        }
    }
}

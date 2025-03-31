using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using XArch.AppShell.Core;

using XArch.AppShell.Framework;

namespace XArch.AppShell
{
    internal static class AppContextInstance
    {
        private static volatile bool _isInitialized = false;
        private static volatile object _lock = new object();

        private static readonly Lazy<Framework.AppContext> _appContext = new Lazy<Framework.AppContext>(() =>
        {
            ServiceProvider services = ConfigureServices();
            return new Framework.AppContext(services);
        });

        public static Framework.AppContext Instance => _appContext.Value;

        private static ServiceProvider ConfigureServices()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddTransient<IAppContext>(services => _appContext.Value);
            IEnumerable<IAtlasStudioPlugin> plugins = PluginLoader.LoadPlugins(serviceDescriptors);
            IEnumerable<ServiceDescriptor> descriptors = plugins.Select(p => ServiceDescriptor.Singleton(p)).ToArray();
            serviceDescriptors.TryAddEnumerable(descriptors);

            return serviceDescriptors.BuildServiceProvider();
        }

        public static void InitializePlugins()
        {
            lock (_lock)
            {
                if (_isInitialized)
                {
                    return;
                }

                IEnumerable<IAtlasStudioPlugin> plugins = Instance.Services.GetServices<IAtlasStudioPlugin>().ToArray();

                // Handle the core plugin first
                plugins.OfType<CorePlugin>().Single().Configure(Instance);

                foreach (IAtlasStudioPlugin plugin in plugins)
                {
                    if (plugin is CorePlugin corePlugin)
                    {
                        continue;
                    }

                    plugin.Configure(Instance);
                }

                _isInitialized = true;
            }
        }

        public static void Dispose()
        {
            _appContext.Value.Dispose();
        }
    }
}

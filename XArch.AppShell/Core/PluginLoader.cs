using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;

namespace XArch.AppShell.Core
{
    public static class PluginLoader
    {
        public static IAtlasStudioPlugin[] LoadPlugins(IServiceCollection services)
        {
            List<IAtlasStudioPlugin> plugins = new List<IAtlasStudioPlugin>();

            var assemblies = GetAssembliesToScan();
            IList<Type> activatedPlugins = new List<Type>();

            plugins.AddRange(LoadPlugins(Assembly.GetExecutingAssembly(), services));

            foreach (var asm in assemblies)
            {
                plugins.AddRange(LoadPlugins(asm, services));
            }

            return plugins.ToArray();
        }

        private static IAtlasStudioPlugin[] LoadPlugins(Assembly assembly, IServiceCollection services)
        {
            List<IAtlasStudioPlugin> plugins = new List<IAtlasStudioPlugin>();

            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IAtlasStudioPlugin).IsAssignableFrom(type) || type.IsAbstract || !type.IsClass)
                    continue;
                var pluginAttr = type.GetCustomAttribute<AtlasStudioPluginAttribute>();
                if (pluginAttr == null || pluginAttr.Enabled == false)
                    continue; // Must be explicitly marked
                try
                {
                    var plugin = (IAtlasStudioPlugin)Activator.CreateInstance(type)!;
                    plugin.RegisterServices(services);
                    plugins.Add(plugin);
                }
                catch (Exception ex)
                {
                    // Optional: log failure to load plugin
                }
            }

            return plugins.ToArray();
        }

        private static IEnumerable<Assembly> GetAssembliesToScan()
        {
            // Load all referenced assemblies of the entry assembly (ensures they're in AppDomain)
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                foreach (var reference in entryAssembly.GetReferencedAssemblies())
                {
                    try
                    {
                        Assembly.Load(reference);
                    }
                    catch
                    {
                        // Optionally log or ignore missing/incompatible assemblies
                    }
                }
            }

            // Load all assemblies in the current assembly folder that match the Kuiper.* pattern
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(path, "XArch.AppShell.*.dll", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    Assembly.LoadFrom(file);
                }
                catch
                {
                    // Optionally log or ignore missing/incompatible assemblies
                }
            }

            // Now return all non-dynamic, resolvable assemblies currently loaded
            var loaded = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location) && a.GetName().Name.StartsWith("XArch.AppShell."))
                .ToList();

            return loaded.Distinct();
        }
    }
}

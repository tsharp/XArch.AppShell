using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;

namespace XArch.AppShell
{
    public static class PluginLoader
    {
        public static void LoadPlugins(IServiceCollection services)
        {
            var assemblies = GetAssembliesToScan();

            foreach (var asm in assemblies)
            {
                foreach (var type in asm.GetTypes())
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
                    }
                    catch (Exception ex)
                    {
                        // Optional: log failure to load plugin
                    }
                }
            }
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

            return loaded;
        }
    }
}

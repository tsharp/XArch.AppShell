using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Events;
using XArch.AppShell.Sdk;

namespace XArch.AppShell.Core
{
    internal class AtlasStudioContext
    {
        public IServiceProvider Services { get; }
        public AtlasProject? CurrentProject { get; private set; }
        public ProjectEventDispatcher ProjectEvents => Services.GetRequiredService<ProjectEventDispatcher>();
        public FileEventDispatcher FileEvents => Services.GetRequiredService<FileEventDispatcher>();

        private AtlasStudioContext()
        {
            Services = ConfigureServices();
        }

        public void Initialize()
        {
            // Do nothing, but this method is useful for future expansion
        }

        private IServiceProvider ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            // configure additional services as needed
            services.AddSingleton<ProjectEventDispatcher>();
            services.AddSingleton<FileEventDispatcher>();

            PluginLoader.LoadPlugins(services);

            return services.BuildServiceProvider();
        }

        private static AtlasStudioContext? globalContext;

        internal static AtlasStudioContext Current => globalContext ??= new AtlasStudioContext();
        public void OpenProject(string location)
        {
            // Cancelable pre-event
            if (!ProjectEvents.RaiseOpening())
                return;

            try
            {
                var project = AtlasProjectService.LoadFromDirectory(location);
                CurrentProject = project;
                ProjectEvents.RaiseOpened(project);
            }
            catch (Exception ex)
            {
                // Optionally: log, notify, or rethrow
                CurrentProject = null;
                // You could raise a ProjectOpenFailed event or log it here
            }
        }

        public void SaveProject()
        {
            if (CurrentProject == null)
                return;

            // Cancelable pre-event
            if (!ProjectEvents.RaiseSaving(CurrentProject))
                return;

            try
            {
                AtlasProjectService.Save(CurrentProject);
                ProjectEvents.RaiseSaved(CurrentProject);
            }
            catch (Exception ex)
            {
                // Optionally: log or rethrow
            }
        }

    }
}

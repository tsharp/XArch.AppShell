using System;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.Menu;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    [AtlasStudioPlugin(nameof(ProjectExplorer), "Provides a control for basic project management functionality.")]
    public class ProjectExplorerPlugin : IAtlasStudioPlugin
    {
        private IServiceProvider? serviceProvider;
        private ProjectExplorer? projectExplorer;

        public void Configure(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.projectExplorer = serviceProvider.GetRequiredService<ProjectExplorer>();

            IMenuManager menuManager = serviceProvider.GetRequiredService<IMenuManager>();
            IViewManager viewManager = serviceProvider.GetRequiredService<IViewManager>();
            IEventManager eventManager = serviceProvider.GetRequiredService<IEventManager>();

            viewManager.RegisterTool(DockLocation.Left, "atlas.project.explorer", "Project Explorer", projectExplorer, canHide: false);
            menuManager.RegisterProvider(new ProjectExplorerMenuProvider(eventManager));

            eventManager.Subscribe("atlas.project.explorer.toggle", _ =>
            {
                viewManager.FocusToolWindow("atlas.project.explorer");
            });

            eventManager.Subscribe("atlas.project.open", @event =>
            {
                viewManager.FocusToolWindow("atlas.project.explorer");
                projectExplorer.LoadProject(@event.Payload?.ToString()!);
            });
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ProjectExplorer>();
        }
    }
}

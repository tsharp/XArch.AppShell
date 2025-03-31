using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    [AtlasStudioPlugin(nameof(ProjectExplorer), "Provides a control for basic project management functionality.")]
    public class ProjectExplorerPlugin : IAtlasStudioPlugin
    {
        private IAppContext _appContext;
        private ProjectExplorer? projectExplorer;

        public void Configure(IAppContext appContext)
        {
            this._appContext = appContext;
            this.projectExplorer = appContext.Services.GetRequiredService<ProjectExplorer>();

            appContext.ViewManager.RegisterTool(DockLocation.Left, "atlas.project.explorer", "Project Explorer", projectExplorer, canHide: false);
            appContext.MenuManager.RegisterProvider(new ProjectExplorerMenuProvider(appContext.EventManager));

            appContext.EventManager.Subscribe("atlas.project.explorer.toggle", _ =>
            {
                appContext.ViewManager.FocusToolWindow("atlas.project.explorer");
            });

            appContext.EventManager.Subscribe("atlas.project.open", @event =>
            {
                appContext.ViewManager.FocusToolWindow("atlas.project.explorer");
                projectExplorer.LoadProject(@event.Payload?.ToString()!);
            });
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ProjectExplorer>();
        }
    }
}

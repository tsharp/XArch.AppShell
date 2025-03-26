using System;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    [AtlasStudioPlugin(nameof(ProjectExplorer), "Provides a control for basic project management functionality.")]
    public class ProjectExplorerPlugin : IAtlasStudioPlugin
    {
        public void Configure(IServiceProvider serviceProvider)
        {
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ProjectExplorer>();
        }
    }
}

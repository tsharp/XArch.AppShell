using System;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Controls.Console
{
    [AtlasStudioPlugin(nameof(ConsoleControl))]
    public class ConsolePlugin : IAtlasStudioPlugin
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            IViewManager viewManager = serviceProvider.GetRequiredService<IViewManager>();
            var consoleControl = serviceProvider.GetRequiredService<ConsoleControl>();
            viewManager.RegisterTool(DockLocation.Bottom, "tool.console", "Console", consoleControl, canHide: true, hiddenByDefault: false);
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ConsoleControl>();
        }
    }
}

using System;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Controls.Console
{
    [AtlasStudioPlugin(nameof(ConsoleControl))]
    public class ConsolePlugin : IAtlasStudioPlugin
    {
        public void Configure(IAppContext appContext)
        {
            var consoleControl = appContext.Services.GetRequiredService<ConsoleControl>();
            appContext.ViewManager.RegisterTool(DockLocation.Bottom, "tool.console", "Console", consoleControl, canHide: true, hiddenByDefault: false);
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ConsoleControl>();
        }
    }
}

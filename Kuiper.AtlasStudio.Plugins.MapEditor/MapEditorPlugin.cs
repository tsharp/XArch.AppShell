using System;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;

namespace Kuiper.AtlasStudio.Plugins.MapEditor
{
    [AtlasStudioPlugin("Markdown Editor Plugin", "Provides basic Markdown editing and preview.")]
    public class MapEditorPlugin : IAtlasStudioPlugin
    {
        public void Configure(IAppContext appContext)
        {
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<MapEditorFactory>();
        }
    }
}

using System;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Markdown
{
    [AtlasStudioPlugin("Markdown Editor Plugin", "Provides basic Markdown editing and preview.")]
    public class MarkdownEditorPlugin : IAtlasStudioPlugin
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            IFileEditorManager editorManager = serviceProvider.GetRequiredService<IFileEditorManager>();
            editorManager.RegisterProvider(serviceProvider.GetRequiredService<MarkdownEditorFactory>());
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<MarkdownEditorFactory>();
        }
    }
}

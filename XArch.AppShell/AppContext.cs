using System;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.Menu;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Framework
{
    internal sealed class AppContext : IAppContext, IDisposable
    {
        private ServiceProvider serivces;
        public IServiceProvider Services { get => serivces; }
        public IMenuManager MenuManager { get => Services.GetRequiredService<IMenuManager>(); }
        public IActiveDocumentProvider ActiveDocumentProvider { get => Services.GetRequiredService<IActiveDocumentProvider>(); }
        public IViewManager ViewManager { get => Services.GetRequiredService<IViewManager>(); }
        public IEventManager EventManager { get => Services.GetRequiredService<IEventManager>(); }
        public IFileEditorManager EditorManager { get => Services.GetRequiredService<IFileEditorManager>(); }

        internal AppContext(ServiceProvider services)
        {
            this.serivces = services;
        }

        public void Dispose()
        {
            serivces.Dispose();
        }
    }
}

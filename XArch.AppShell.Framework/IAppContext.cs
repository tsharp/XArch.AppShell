using System;

using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.Menu;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Framework
{
    public interface IAppContext
    {
        public IServiceProvider Services { get; }
        public IMenuManager MenuManager { get; }
        public IActiveDocumentProvider ActiveDocumentProvider { get; }
        public IViewManager ViewManager { get; }
        public IEventManager EventManager { get; }
        public IFileEditorManager EditorManager { get; }
    }
}

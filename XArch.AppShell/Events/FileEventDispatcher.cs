using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.Events
{
    public class FileEventDispatcher : StudioEventDispatcherBase<IFileEventListener>
    {
        public FileEventDispatcher(IServiceProvider services) : base(services) { }

        public void RaiseOpening(string path)
        {
            foreach (var listener in Listeners)
                listener.OnFileOpening(path);
        }

        public void RaiseOpened(string path)
        {
            foreach (var listener in Listeners)
                listener.OnFileOpened(path);
        }

        public void RaiseClosing(string path)
        {
            foreach (var listener in Listeners)
                listener.OnFileClosing(path);
        }

        public void RaiseClosed(string path)
        {
            foreach (var listener in Listeners)
                listener.OnFileClosed(path);
        }

        public void RaiseSaving(string path)
        {
            foreach (var listener in Listeners)
                listener.OnFileSaving(path);
        }

        public void RaiseSaved(string path)
        {
            foreach (var listener in Listeners)
                listener.OnFileSaved(path);
        }
    }

}

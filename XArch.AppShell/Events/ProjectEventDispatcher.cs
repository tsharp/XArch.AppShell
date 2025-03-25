using XArch.AppShell.Framework.Events;
using XArch.AppShell.Sdk;

namespace XArch.AppShell.Events
{
    public class ProjectEventDispatcher : StudioEventDispatcherBase<IProjectEventListener>
    {
        public ProjectEventDispatcher(IServiceProvider services) : base(services) { }

        public bool RaiseOpening()
        {
            return TryInvokeCancelable(Listeners, (l, ctx) => l.OnProjectOpening(ctx), Framework.Void.Instance);
        }

        public void RaiseOpened(AtlasProject project)
        {
            foreach (var listener in Listeners)
                listener.OnProjectOpened(project);
        }

        public bool RaiseClosing(AtlasProject project)
        {
            return TryInvokeCancelable(Listeners, (l, ctx) => l.OnProjectClosing(ctx), project);
        }

        public void RaiseClosed(AtlasProject project)
        {
            foreach (var listener in Listeners)
                listener.OnProjectClosed(project);
        }

        public bool RaiseSaving(AtlasProject project)
        {
            return TryInvokeCancelable(Listeners, (l, ctx) => l.OnProjectSaving(ctx), project);
        }

        public void RaiseSaved(AtlasProject project)
        {
            foreach (var listener in Listeners)
                listener.OnProjectSaved(project);
        }
    }

}

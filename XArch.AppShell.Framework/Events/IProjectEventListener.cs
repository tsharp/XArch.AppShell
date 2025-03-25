using XArch.AppShell.Sdk;

namespace XArch.AppShell.Framework.Events
{
    public interface IProjectEventListener
    {
        void OnProjectOpening(CancelableEventContext<Void>? ctx);
        void OnProjectOpened(AtlasProject project);
        void OnProjectClosing(CancelableEventContext<AtlasProject> ctx);
        void OnProjectClosed(AtlasProject project);
        void OnProjectSaving(CancelableEventContext<AtlasProject> ctx);
        void OnProjectSaved(AtlasProject project);
    }
}

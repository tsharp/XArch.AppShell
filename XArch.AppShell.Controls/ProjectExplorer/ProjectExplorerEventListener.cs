using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Sdk;

namespace XArch.AppShell.Controls.ProjectExplorer
{
    public sealed class ProjectExplorerEventListener : IProjectEventListener
    {
        public void OnProjectClosed(AtlasProject project)
        {
            throw new System.NotImplementedException();
        }

        public void OnProjectClosing(CancelableEventContext<AtlasProject> ctx)
        {
            throw new System.NotImplementedException();
        }

        public void OnProjectOpened(AtlasProject project)
        {
            throw new System.NotImplementedException();
        }

        public void OnProjectOpening(CancelableEventContext<Void>? ctx)
        {
            throw new System.NotImplementedException();
        }

        public void OnProjectSaved(AtlasProject project)
        {
            throw new System.NotImplementedException();
        }

        public void OnProjectSaving(CancelableEventContext<AtlasProject> ctx)
        {
            throw new System.NotImplementedException();
        }
    }
}

namespace XArch.AppShell.Framework.UI
{
    public interface IViewManager
    {
        void OpenDocument(string contentId, string title, object content);
        void RegisterTool(DockSide side, string contentId, string title, object content, bool canHide = true, bool hiddenByDefault = true);
        void FocusDocument(string contentId);
        void FocusToolWindow(string contentId);
    }
}

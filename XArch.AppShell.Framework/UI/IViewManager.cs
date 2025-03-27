namespace XArch.AppShell.Framework.UI
{
    public interface IViewManager
    {
        void OpenDocument(string contentId, string title, EditorControl control);
        void RegisterTool(DockSide side, string contentId, string title, object control, bool canHide = true, bool hiddenByDefault = true);
        void FocusDocument(string contentId);
        void FocusToolWindow(string contentId);
    }
}

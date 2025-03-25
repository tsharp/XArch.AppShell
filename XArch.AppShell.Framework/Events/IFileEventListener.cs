namespace XArch.AppShell.Framework.Events
{
    public interface IFileEventListener
    {
        void OnFileOpening(string relativePath);
        void OnFileOpened(string relativePath);
        void OnFileClosing(string relativePath);
        void OnFileClosed(string relativePath);
        void OnFileSaving(string relativePath);
        void OnFileSaved(string relativePath);
    }

}

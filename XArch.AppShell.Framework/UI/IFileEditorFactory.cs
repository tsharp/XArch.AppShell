namespace XArch.AppShell.Framework.UI
{
    public interface IFileEditorFactory
    {
        bool CanOpen(string fileExtension);
        EditorControl Create(string filePath);
    }
}

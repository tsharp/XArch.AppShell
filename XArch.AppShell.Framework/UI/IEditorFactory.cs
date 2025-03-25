using System.Windows;

namespace XArch.AppShell.Framework.UI
{
    public interface IEditorFactory
    {
        bool CanOpen(string fileExtension);
        IEditorView Create(string filePath);
    }
}

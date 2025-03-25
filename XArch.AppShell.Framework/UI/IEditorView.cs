using System.Windows;

namespace XArch.AppShell.Framework.UI
{
    public interface IEditorView
    {
        string FilePath { get; }
        UIElement GetControl(); // Usually returns a WPF UserControl
    }
}

using System.Windows;

using XArch.AppShell.Markdown;

using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Markdown
{
    public class MarkdownEditorView : IEditorView
    {
        public string FilePath { get; }

        public MarkdownEditorView(string filePath)
        {
            FilePath = filePath;
        }

        public UIElement GetControl()
        {
            return new MarkdownEditorControl(FilePath);
        }
    }

}

using System;

using XArch.AppShell.Markdown;

using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Markdown
{
    public class MarkdownEditorFactory : IEditorFactory
    {
        public bool CanOpen(string fileExtension)
        {
            return fileExtension.Equals(".md", StringComparison.OrdinalIgnoreCase);
        }

        public IEditorView Create(string filePath)
        {
            return new MarkdownEditorView(filePath);
        }
    }

}

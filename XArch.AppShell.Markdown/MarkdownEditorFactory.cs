using System;

using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Markdown
{
    public class MarkdownEditorFactory : IFileEditorFactory
    {
        private readonly IEventManager _eventManager;

        public MarkdownEditorFactory(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public bool CanOpen(string fileExtension)
        {
            return fileExtension.Equals(".md", StringComparison.OrdinalIgnoreCase);
        }

        public EditorControl Create(string filePath)
        {
            return new MarkdownEditorControl(_eventManager, filePath);
        }
    }

}

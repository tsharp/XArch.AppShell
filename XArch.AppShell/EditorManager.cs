using System.IO;

using XArch.AppShell.Framework.UI;

namespace XArch.AppShell
{
    public class EditorManager
    {
        private readonly List<IEditorFactory> _factories = new();

        public void RegisterFactory(IEditorFactory factory) => _factories.Add(factory);

        public IEditorView? CreateEditor(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            var factory = _factories.FirstOrDefault(f => f.CanOpen(ext));
            return factory?.Create(filePath);
        }
    }

}

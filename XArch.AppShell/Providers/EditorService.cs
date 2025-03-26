using System.IO;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Providers
{
    public class EditorService : ProviderManagerBase<IEditorFactory>, IEditorService
    {
        private readonly IDictionary<string, IEditorView> openEditors = new Dictionary<string, IEditorView>();

        public IEditorView? CreateEditor(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            var factory = _providers.FirstOrDefault(f => f.CanOpen(ext));
            return factory?.Create(filePath);
        }
    }
}

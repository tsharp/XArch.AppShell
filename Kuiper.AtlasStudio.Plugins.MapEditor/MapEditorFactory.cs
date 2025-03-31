using System;

using XArch.AppShell.Framework.UI;

namespace Kuiper.AtlasStudio.Plugins.MapEditor
{
    internal class MapEditorFactory : IFileEditorFactory
    {
        public bool CanOpen(string fileExtension)
        {
            return false;
        }

        public EditorControl Create(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}

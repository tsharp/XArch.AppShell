using System;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.UI;
using XArch.AppShell.TileEditor.Controls;

namespace XArch.AppShell.TileEditor
{
    internal class TileEditorFactory : IFileEditorFactory
    {
        private readonly IAppContext _appContext;
        private readonly TileBrushTool _tileBrushTool;

        public TileEditorFactory(IAppContext appContext, TileBrushTool tileBrushTool)
        {
            _appContext = appContext;
            _tileBrushTool = tileBrushTool;
        }

        /// <summary>
        /// kmapx is a self-contained file format that contains all the information needed to render a tile map.
        /// kmap is a text-based file format that contains the same information as kmapx, but is human-readable.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public bool CanOpen(string fileExtension)
        {
            return fileExtension.Equals(".kmap", StringComparison.OrdinalIgnoreCase);
        }

        public EditorControl Create(string filePath)
        {
            return new TileEditorControl(_tileBrushTool, _appContext.EventManager, filePath);
        }
    }

}

using System;

using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.UI;
using XArch.AppShell.TileEditor.Controls;

namespace XArch.AppShell.TileEditor
{
    internal class MapEditorFactory : IFileEditorFactory
    {
        private readonly IEventManager _eventManager;
        private readonly TileBrushTool _tileBrushTool;

        public MapEditorFactory(TileBrushTool tileBrushTool, IEventManager eventManager)
        {
            _eventManager = eventManager;
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
            return fileExtension.Equals(".kmap_old", StringComparison.OrdinalIgnoreCase);
        }

        public EditorControl Create(string filePath)
        {
            return new MapEditorControl(_tileBrushTool, _eventManager, filePath);
        }
    }

}

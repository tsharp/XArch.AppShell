using System.Collections.Generic;

namespace XArch.AppShell.Framework.Menu
{
    public interface IMenuProvider
    {
        string ContextMenuId { get; }
        MenuType MenuType { get; }
        IEnumerable<MenuEntry> GetMenuItems();
    }
}

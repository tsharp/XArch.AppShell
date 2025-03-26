using System.Collections.Generic;

namespace XArch.AppShell.Framework.Menu
{
    public interface IMenuProvider
    {
        IEnumerable<MenuEntry> GetMenus();
    }
}

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Menu;

namespace XArch.AppShell.Core
{
    internal class CoreMainMenuProvider : IMenuProvider
    {
        public MenuType MenuType => MenuType.MainMenu;

        public string ContextMenuId => throw new NotImplementedException();

        public IEnumerable<MenuEntry> GetMenuItems()
        {
            yield return new MenuEntry("_File", new[]
            {
                new MenuEntry("New", new[]
                {
                    new MenuEntry("Project"),
                    new MenuEntry("File")
                }),
                new MenuEntry("Open"),
                new MenuEntry() { IsSeparator = true },
                new MenuEntry("Save"),
                new MenuEntry("Save As"),
                new MenuEntry() { IsSeparator = true },
                new MenuEntry("Exit")
                {
                    Command = new RelayCommand<object>(_ => System.Windows.Application.Current.Shutdown())
                }
            });

            yield return new MenuEntry("_Edit", new[]
            {
                new MenuEntry("Undo"),
                new MenuEntry("Redo"),
                new MenuEntry("Cut"),
                new MenuEntry("Copy"),
                new MenuEntry("Paste")
            });

            yield return new MenuEntry("_View", new[]
            {
                new MenuEntry("Solution Explorer"),
                new MenuEntry("Properties")
            });

            yield return new MenuEntry("_Help", new[]
            {
                new MenuEntry("About")
            });
        }
    }
}

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Menu;

namespace XArch.AppShell.Core
{
    internal class CoreMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuEntry> GetMenus()
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
                    Command = new RelayCommand((e) => System.Windows.Application.Current.Shutdown())
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

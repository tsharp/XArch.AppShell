using System.Windows.Controls;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Menu;

namespace XArch.AppShell.Providers
{
    internal class MenuManager : ProviderManagerBase<IMenuProvider>, IMenuManager
    {
        public void Build(Menu targetMenu)
        {
            targetMenu.Items.Clear();

            var mergedTopLevelMenus = MergeMenuEntries(_providers.SelectMany(p => p.GetMenus()));

            foreach (var topLevel in mergedTopLevelMenus)
            {
                var menuItem = new MenuItem
                {
                    Header = topLevel.Header,
                    Command = topLevel.Command,
                };

                foreach (var child in topLevel.Children.OrderBy(c => c.Order))
                {
                    menuItem.Items.Add(CreateMenuItem(child));
                }

                targetMenu.Items.Add(menuItem);
            }
        }

        private static object CreateMenuItem(MenuEntry entry)
        {
            if (entry.IsSeparator)
                return new Separator();

            var item = new MenuItem
            {
                Header = entry.Header,
                Command = entry.Command
            };

            foreach (var child in entry.Children.OrderBy(c => c.Order))
                item.Items.Add(CreateMenuItem(child));

            return item;
        }

        private static List<MenuEntry> MergeMenuEntries(IEnumerable<MenuEntry> entries)
        {
            static string NormalizeHeader(string header) => header.Replace("_", "").Trim();

            List<MenuEntry> result = new();

            foreach (var entry in entries)
            {
                var normalized = NormalizeHeader(entry.Header);

                var existing = result.FirstOrDefault(e =>
                    NormalizeHeader(e.Header).Equals(normalized, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    // Deep clone with merged children
                    result.Add(new MenuEntry
                    {
                        Header = entry.Header,
                        Order = entry.Order,
                        Command = entry.Command,
                        IsSeparator = entry.IsSeparator,
                        Children = MergeMenuEntries(entry.Children)
                    });
                }
                else
                {
                    // Prefer underscore version
                    if (!existing.Header.Contains("_") && entry.Header.Contains("_"))
                        existing.Header = entry.Header;

                    // Merge children recursively
                    var mergedChildren = MergeMenuEntries(existing.Children.Concat(entry.Children));
                    existing.Children = mergedChildren;

                    // Keep lower order and first non-null command
                    existing.Order = Math.Min(existing.Order, entry.Order);
                    if (existing.Command == null && entry.Command != null)
                        existing.Command = entry.Command;
                }
            }

            return result.OrderBy(e => e.Order).ToList();
        }
    }

}

using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Menu;

namespace XArch.AppShell.Providers
{
    internal class MenuManager : ProviderManagerBase<IMenuProvider>, IMenuManager
    {
        public void BuildMainMenu(Menu targetMenu)
        {
            targetMenu.Items.Clear();
            targetMenu.IsMainMenu = true;

            var mergedTopLevelMenus = MergeMenuEntries(_providers.Where(p => p.MenuType == MenuType.MainMenu).SelectMany(p => p.GetMenuItems()));

            foreach (var topLevel in mergedTopLevelMenus)
            {
                var menuItem = new MenuItem
                {
                    Header = topLevel.Header,
                    Command = topLevel.Command,
                };

                foreach (var child in topLevel.Children.OrderBy(c => c.Order))
                {
                    menuItem.Items.Add(CreateMenuItem(child, false));
                }

                targetMenu.Items.Add(menuItem);
            }

            if (targetMenu.Items.Count == 0)
            {
                targetMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                targetMenu.Visibility = Visibility.Visible;
            }
        }

        public static Separator CreateSeparator(bool isHorizontal)
        {
            if (isHorizontal)
            {
                // Create a horizontal separator
                Separator separator = new Separator()
                {
                    Width = 1,
                    Height = double.NaN
                };
                // separator.Width = double.NaN;  // Width should be automatically adjusted for horizontal
                // separator.Height = 1;  // Set a fixed height for horizontal separator (you can change the thickness here)
                return separator;
            }

            // Create a vertical separator
            return new Separator();
        }

        private object CreateMenuControl(MenuEntry entry)
        {
            // This is intended to be a special control
            if (entry.ControlType != null && (entry.Children == null || entry.Children.Count == 0))
            {
                return App.Context.Services.GetRequiredService(entry.ControlType);
            }
            else if (entry.ControlType != null)
            {
                throw new InvalidOperationException("ControlType is only supported for leaf menu items");
            }

            return null;
        }

        private object CreateMenuItem(MenuEntry entry, bool isHorizontal)
        {
            if (entry.IsSeparator)
                return CreateSeparator(isHorizontal);

            // This is intended to be a special control
            object menuControl = CreateMenuControl(entry);

            if (menuControl != null)
                return menuControl;

            var item = new MenuItem
            {
                Header = entry.Header,
                Command = entry.Command
            };

            foreach (var child in entry.Children.OrderBy(c => c.Order))
                item.Items.Add(CreateMenuItem(child, isHorizontal));

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

                if (existing == null || entry.IsSeparator || entry.ControlType != null)
                {
                    // Deep clone with merged children
                    result.Add(new MenuEntry
                    {
                        ControlType = entry.ControlType,
                        Header = entry.Header,
                        Order = entry.Order,
                        Command = entry.IsSeparator ? null : entry.Command,
                        IsSeparator = entry.IsSeparator,
                        Children = entry.IsSeparator ? new List<MenuEntry>() : MergeMenuEntries(entry.Children)
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

        private TabItem CreateRibbonTab(MenuEntry menuEntry)
        {
            object menuControl = CreateMenuControl(menuEntry);

            return new TabItem()
            {
                Header = menuEntry.Header,
                Content = menuControl ?? new Menu()
                {
                    AllowDrop = false,
                    FlowDirection = FlowDirection.LeftToRight
                }
            };
        }

        internal void BuildToolbarMenu(TabControl appRibbon)
        {
            appRibbon.Items.Clear();

            var mergedTopLevelMenus = MergeMenuEntries(_providers.Where(p => p.MenuType == MenuType.Toolbar).SelectMany(p => p.GetMenuItems()));

            foreach (var topLevel in mergedTopLevelMenus)
            {
                if (topLevel.Command != null)
                {
                    throw new InvalidOperationException("Top level menu entries in toolbar must not have a command");
                }

                var ribbonTab = CreateRibbonTab(topLevel);
                var ribbonMenu = ribbonTab.Content as Menu;

                foreach (var child in topLevel.Children.OrderBy(c => c.Order))
                {
                    ribbonMenu.Items.Add(CreateMenuItem(child, true));
                }

                appRibbon.Items.Add(ribbonTab);
            }

            if (appRibbon.Items.Count == 0)
            {
                appRibbon.Visibility = Visibility.Collapsed;
            }
            else
            {
                appRibbon.Visibility = Visibility.Visible;
            }
        }
    }
}

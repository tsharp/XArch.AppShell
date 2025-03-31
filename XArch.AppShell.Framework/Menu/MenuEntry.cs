using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace XArch.AppShell.Framework.Menu
{
    public class MenuEntry
    {
        public MenuEntry() { }

        public MenuEntry(string header, ICommand? command = null, bool isSeparator = false, int order = 0)
        {
            Header = header;
            Command = command;
            IsSeparator = isSeparator;
            Order = order;
        }

        public MenuEntry(string header, IEnumerable<MenuEntry> children, bool isSeparator = false, int order = 0)
        {
            Header = header;
            Children = new List<MenuEntry>(children);
            IsSeparator = isSeparator;
            Order = order;
        }

        public Type? ControlType { get; set; } = null;
        public string Icon { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public ICommand? Command { get; set; }
        public bool IsSeparator { get; set; } = false;
        public int Order { get; set; } = 0;
        public List<MenuEntry> Children { get; set; } = new List<MenuEntry>();
    }
}

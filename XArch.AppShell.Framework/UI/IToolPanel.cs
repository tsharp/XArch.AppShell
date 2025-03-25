namespace XArch.AppShell.Framework.UI
{
    using System.Windows;

    public interface IToolPanel
    {
        string Title { get; }
        UIElement GetControl();
    }

}

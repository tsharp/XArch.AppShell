using System.Windows;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Providers;

namespace XArch.AppShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Startup();
            PreviewKeyDown += OnPreviewKeyDown;
        }

        private void Startup()
        {
            // Build the menu.
            MenuManager menuManager = App.Context.Services.GetRequiredService<MenuManager>();
            menuManager.BuildMainMenu(AppMenu);
            menuManager.BuildToolbarMenu(AppToolbar);

            // Build and set the dock view manager.
            DockViewManager viewManager = App.Context.Services.GetRequiredService<DockViewManager>();
            AppDock.Content = viewManager.DockingManager;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                IEventManager eventManager = App.Context.Services.GetRequiredService<IEventManager>();
                eventManager.Publish("project.file.save_active");
            }
        }
    }
}
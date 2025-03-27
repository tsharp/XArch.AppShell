using System.Windows;

using Microsoft.Extensions.DependencyInjection;

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
        }

        private void Startup()
        {
            MenuManager menuManager = App.Services.GetRequiredService<MenuManager>();
            menuManager.Build(AppMenu);
        }
    }
}
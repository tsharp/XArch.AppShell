using System.Windows.Controls;

using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Providers;

namespace XArch.AppShell
{
    /// <summary>
    /// Interaction logic for AppShellDock.xaml
    /// </summary>
    public partial class AppShellDock : UserControl
    {
        public AppShellDock()
        {
            InitializeComponent();

            // This truly initializes the DockingManager component.
            DockViewManager viewManager = App.Services.GetRequiredService<DockViewManager>();
            this.Content = viewManager.DockingManager;
        }
    }
}

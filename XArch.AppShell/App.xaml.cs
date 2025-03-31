using System.Windows;

namespace XArch.AppShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static Framework.AppContext Context => AppContextInstance.Instance;

        public App()
        {
            this.Exit += App_OnExit;
            AppContextInstance.InitializePlugins();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            AppContextInstance.Dispose();
        }
    }
}

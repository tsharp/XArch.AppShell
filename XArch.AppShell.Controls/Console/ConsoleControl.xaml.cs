using System.Windows;
using System.Windows.Controls;

using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.Controls.Console
{
    /// <summary>
    /// Interaction logic for ConsoleControl.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl
    {
        public ConsoleViewModel ViewModel { get; } = new();

        public ConsoleControl(IEventManager eventManager)
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.Events.CollectionChanged += (_, __) =>
            {
                ConsoleScrollViewer?.ScrollToEnd();
            };

            eventManager.Subscribe("*", @event => Log($"{@event.Name}: {@event.Payload}"));
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Clear();
        }

        public void Log(string message)
        {
            ViewModel.Add(message);
        }
    }
}

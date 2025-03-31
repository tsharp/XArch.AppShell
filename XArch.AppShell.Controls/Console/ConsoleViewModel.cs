using System.Collections.ObjectModel;
using System.ComponentModel;

namespace XArch.AppShell.Controls.Console
{
    public class ConsoleViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ConsoleEvent> Events { get; } = new();

        public void Add(string message)
        {
            Events.Add(new ConsoleEvent { Message = message });
            OnPropertyChanged(nameof(Events));
        }

        public void Clear()
        {
            Events.Clear();
            OnPropertyChanged(nameof(Events));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}

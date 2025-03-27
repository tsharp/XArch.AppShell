using System;
using System.Windows.Input;

namespace XArch.AppShell.Framework
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }
        public void Execute(object? parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler? CanExecuteChanged;
    }
}

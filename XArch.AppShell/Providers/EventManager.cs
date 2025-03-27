using System.Windows.Threading;

using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.Providers
{
    internal class EventManager : IEventManager
    {
        private readonly Dictionary<string, List<Action<Event>>> _handlers = new();

        public void Publish(string name, object? payload = null)
        {
            var evt = new Event(name, payload);
            Dispatcher.CurrentDispatcher.BeginInvoke(() =>
            {
                // Will run *after* current call stack finishes
                // UI is NOT blocked while waiting for this
                if (_handlers.TryGetValue(name, out var handlers))
                {
                    foreach (var handler in handlers)
                    {
                        handler.Invoke(evt);
                        if (evt.Canceled) break; // Stop further handling if the event was canceled
                    }
                }
            }).Wait();
        }

        public void Subscribe(string name, Action<Event> handler)
        {
            if (!_handlers.TryGetValue(name, out var handlers))
            {
                handlers = new List<Action<Event>>();
                _handlers[name] = handlers;
            }

            handlers.Add(handler);
        }

        public void Unsubscribe(string name, Action<Event> handler)
        {
            if (_handlers.TryGetValue(name, out var handlers))
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                    _handlers.Remove(name);
            }
        }
    }

}

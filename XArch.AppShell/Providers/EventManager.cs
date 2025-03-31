using System.Text.RegularExpressions;
using System.Windows.Threading;

using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.Providers
{
    internal class EventManager : IEventManager
    {
        private readonly Dictionary<string, List<Action<Event>>> _handlers = new();
        private readonly Dictionary<string, Regex> _compiledPatterns = new(); // Cache compiled regex

        public void Publish(string name, object? payload = null)
        {
            var evt = new Event(name, payload);
            Dispatcher.CurrentDispatcher.BeginInvoke(() =>
            {
                foreach (var (pattern, handlers) in _handlers)
                {
                    if (IsMatch(name, pattern))
                    {
                        foreach (var handler in handlers)
                        {
                            handler.Invoke(evt);
                            if (evt.Canceled) return; // Stop further handling if canceled
                        }
                    }
                }
            }).Wait();
        }

        public void Subscribe(string pattern, Action<Event> handler)
        {
            if (!_handlers.TryGetValue(pattern, out var handlers))
            {
                handlers = new List<Action<Event>>();
                _handlers[pattern] = handlers;
                _compiledPatterns[pattern] = GlobToRegex(pattern);
            }

            handlers.Add(handler);
        }

        public void Unsubscribe(string pattern, Action<Event> handler)
        {
            if (_handlers.TryGetValue(pattern, out var handlers))
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    _handlers.Remove(pattern);
                    _compiledPatterns.Remove(pattern);
                }
            }
        }

        private bool IsMatch(string name, string pattern)
        {
            if (_compiledPatterns.TryGetValue(pattern, out var regex))
                return regex.IsMatch(name);
            return false;
        }

        private static Regex GlobToRegex(string pattern)
        {
            var escaped = Regex.Escape(pattern);
            var regexPattern = "^" + escaped.Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
            return new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}

using System;

namespace XArch.AppShell.Framework.Events
{
    public interface IEventManager
    {
        void Publish(string name, object? payload = null);
        void Subscribe(string name, Action<Event> handler);
        void Unsubscribe(string name, Action<Event> handler);
    }
}

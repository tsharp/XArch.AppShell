using System;

namespace XArch.AppShell.Framework.Events
{
    public interface IEventManager
    {
        // One flaw is that when the event is published - there's no way to know if it was handled or not.
        // From the publisher's perspective, it's fire and forget.
        void Publish(string name, object? payload = null);
        void Subscribe(string name, Action<Event> handler);
        void Unsubscribe(string name, Action<Event> handler);
    }
}

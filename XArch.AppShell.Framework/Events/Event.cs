using System;

namespace XArch.AppShell.Framework.Events
{
    public class Event
    {
        public string Name { get; }
        public object? Payload { get; }

        public Event(string name, object? payload = null, bool canCancel = false)
        {
            CanCancel = canCancel;
            Name = name;
            Payload = payload;
        }

        public readonly bool CanCancel;

        public bool Canceled { get; private set; }

        public void Cancel()
        {
            if (!CanCancel)
            {
                throw new InvalidOperationException("This event cannot be canceled.");
            }

            Canceled = true;
        }
    }
}

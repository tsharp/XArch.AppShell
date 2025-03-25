using Microsoft.Extensions.DependencyInjection;

using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.Events
{
    public abstract class StudioEventDispatcherBase<TListener>
    {
        private readonly IServiceProvider _services;

        protected StudioEventDispatcherBase(IServiceProvider services)
        {
            _services = services;
        }

        protected IEnumerable<TListener> Listeners =>
            _services.GetServices<TListener>();

        protected bool TryInvokeCancelable<TData>(
            IEnumerable<TListener> listeners,
            Action<TListener, CancelableEventContext<TData>> action,
            TData data)
        {
            var context = new CancelableEventContext<TData>(data);
            foreach (var listener in listeners)
            {
                try
                {
                    action(listener, context);
                    if (context.Cancel)
                        return false;
                }
                catch
                {
                    return false; // Abort on any exception
                }
            }

            return true;
        }

        protected bool TryInvoke<TData>(
            IEnumerable<TListener> listeners,
            Action<TListener, TData> action,
            TData data)
        {
            foreach (var listener in listeners)
            {
                try
                {
                    action(listener, data);
                }
                catch
                {
                    // Optionally log exception
                }
            }

            return true;
        }

    }
}

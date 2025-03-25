namespace XArch.AppShell.Framework.Events
{
    public class CancelableEventContext<T>
    {
        public T Data { get; }
        public bool Cancel { get; private set; } = false;

        public CancelableEventContext(T data)
        {
            Data = data;
        }

        public void Abort() => Cancel = true;
    }

}

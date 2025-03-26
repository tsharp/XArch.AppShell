namespace XArch.AppShell.Framework
{
    public interface IProviderManager<TProvider>
    {
        void RegisterProvider(TProvider provider);
    }
}

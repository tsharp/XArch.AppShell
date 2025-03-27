namespace XArch.AppShell.Framework
{
    public interface IProviderManager<TProvider>
    {
        void RegisterProvider(TProvider provider);
        void RegisterProvider<TInstancedProvider>() where TInstancedProvider : TProvider, new();
    }
}

using System.Collections.Generic;

namespace XArch.AppShell.Framework
{
    public abstract class ProviderManagerBase<TProvider> : IProviderManager<TProvider>
    {
        protected readonly List<TProvider> _providers = new List<TProvider>();

        public void RegisterProvider(TProvider provider) => _providers.Add(provider);
    }
}

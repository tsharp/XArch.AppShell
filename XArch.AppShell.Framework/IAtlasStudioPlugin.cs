using Microsoft.Extensions.DependencyInjection;

namespace XArch.AppShell.Framework
{
    public interface IAtlasStudioPlugin
    {
        void RegisterServices(IServiceCollection services);
    }
}

using System;

using Microsoft.Extensions.DependencyInjection;

namespace XArch.AppShell.Framework
{
    public interface IAtlasStudioPlugin
    {
        void RegisterServices(IServiceCollection services);
        void Configure(IServiceProvider serviceProvider);
    }
}

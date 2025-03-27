using Microsoft.Extensions.DependencyInjection;

namespace XArch.AppShell.Core
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInternalSingleton<TPublicType, TInternalType>(this IServiceCollection services)
            where TInternalType : class, TPublicType
        {
            services.AddSingleton<TInternalType>();
            services.AddTransient(typeof(TPublicType), services =>
            {
                TInternalType internalService = services.GetRequiredService<TInternalType>();

                return internalService;
            });

            return services;
        }
    }
}

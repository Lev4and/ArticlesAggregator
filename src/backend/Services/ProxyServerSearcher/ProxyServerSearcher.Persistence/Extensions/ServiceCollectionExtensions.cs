using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerSearcher.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        return services;
    }
}
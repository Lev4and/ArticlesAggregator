using Microsoft.Extensions.DependencyInjection;
using ProxyServerAggregator.Application.Abstracts.ProxyServers;

namespace ProxyServerAggregator.Infrastructure.ProxyServers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxyServers(this IServiceCollection services)
    {
        services.AddScoped<IProxyServerService, ProxyServerService>();
        services.AddScoped<IProxyServerCacheService, ProxyServerCacheService>();
        services.AddScoped<IProxyServerTestRequestService, ProxyServerTestRequestService>();
        
        return services;
    }
}
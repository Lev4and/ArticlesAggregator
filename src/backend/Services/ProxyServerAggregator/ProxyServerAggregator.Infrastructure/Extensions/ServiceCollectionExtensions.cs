using Caching.Redis.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ProxyServerAggregator.Infrastructure.Messaging.Pulsar.Extensions;
using ProxyServerAggregator.Infrastructure.ProxyServers.Extensions;

namespace ProxyServerAggregator.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddRedisMemoryCache();
        services.AddPulsarMessaging();
        services.AddProxyServers();
        
        return services;
    }
}
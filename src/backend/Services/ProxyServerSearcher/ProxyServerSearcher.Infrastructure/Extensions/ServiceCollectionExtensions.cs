using Microsoft.Extensions.DependencyInjection;
using ProxyServerSearcher.Infrastructure.Messaging.Pulsar.Extensions;
using ProxyServerSearcher.Infrastructure.ProxyServers.Extensions;

namespace ProxyServerSearcher.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddPulsarMessaging();
        services.AddProxyServers();
        
        return services;
    }
}
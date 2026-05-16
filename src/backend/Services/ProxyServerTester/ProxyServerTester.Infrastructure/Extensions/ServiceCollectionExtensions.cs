using Microsoft.Extensions.DependencyInjection;
using ProxyServerTester.Infrastructure.Messaging.Pulsar.Extensions;
using ProxyServerTester.Infrastructure.ProxyServers.Extensions;

namespace ProxyServerTester.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddPulsarMessaging();
        services.AddProxyServers();
        
        return services;
    }
}
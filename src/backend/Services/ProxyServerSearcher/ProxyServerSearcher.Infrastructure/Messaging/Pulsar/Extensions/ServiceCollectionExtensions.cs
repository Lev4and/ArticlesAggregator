using Messaging.Pulsar.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerSearcher.Infrastructure.Messaging.Pulsar.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPulsarMessaging(this IServiceCollection services)
    {
        services.AddPulsarConfiguration();
        services.AddPulsarTopicConfiguration();
        services.AddPulsarMessaging(typeof(ServiceCollectionExtensions).Assembly);
        
        return services;
    }
}
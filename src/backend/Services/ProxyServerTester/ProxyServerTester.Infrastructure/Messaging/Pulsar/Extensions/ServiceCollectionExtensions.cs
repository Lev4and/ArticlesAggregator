using DotPulsar;
using Messaging.Abstracts;
using Messaging.Outbox.Handling.Extensions;
using Messaging.Pulsar.Configurations;
using Messaging.Pulsar.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerTester.Infrastructure.Messaging.Pulsar.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPulsarMessaging(this IServiceCollection services)
    {
        services.AddPulsarConfiguration();
        services.AddPulsarTopicConfiguration();
        services.AddPulsarMessaging(typeof(ServiceCollectionExtensions).Assembly);

        services.AddMessagingOutboxHandling();

        services.AddPulsarConsumer(
            new PulsarConsumerConfiguration
            {
                Topic            = MessageTopic.ProxyServerTestEvents,
                SubscriptionType = SubscriptionType.KeyShared,
                #if DEBUG        
                SubscriptionName = "proxy-server-tester-sub-debug",
                #else
                SubscriptionName = "proxy-server-tester-sub",
                #endif
                InitialPosition  = SubscriptionInitialPosition.Earliest,
                Count            = 2
            });

        return services;
    }
}
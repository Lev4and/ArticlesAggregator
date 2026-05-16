using DotPulsar;
using Messaging.Abstracts;
using Messaging.Outbox.Handling.Extensions;
using Messaging.Pulsar.Configurations;
using Messaging.Pulsar.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerAggregator.Infrastructure.Messaging.Pulsar.Extensions;

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
                Topic            = MessageTopic.ProxyServerEvents,
                SubscriptionType = SubscriptionType.KeyShared,
                #if DEBUG        
                SubscriptionName = "proxy-server-aggregator-sub-debug",
                #else
                SubscriptionName = "proxy-server-aggregator-sub",
                #endif
                InitialPosition  = SubscriptionInitialPosition.Earliest,
                Count            = 2
            });
        
        services.AddPulsarConsumer(
            new PulsarConsumerConfiguration
            {
                Topic            = MessageTopic.ProxyServerTestResultEvents,
                SubscriptionType = SubscriptionType.KeyShared,
                #if DEBUG        
                SubscriptionName = "proxy-server-aggregator-sub-debug",
                #else
                SubscriptionName = "proxy-server-aggregator-sub",
                #endif
                InitialPosition  = SubscriptionInitialPosition.Earliest,
                Count            = 2
            });

        return services;
    }
}
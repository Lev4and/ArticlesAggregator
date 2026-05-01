using DotPulsar;

namespace Messaging.Pulsar.Configurations;

public interface IPulsarConsumerConfiguration
{
    string Topic { get; }
    
    SubscriptionType SubscriptionType { get; }

    string SubscriptionName { get; }

    SubscriptionInitialPosition InitialPosition { get; }
    
    int Count { get; }
}
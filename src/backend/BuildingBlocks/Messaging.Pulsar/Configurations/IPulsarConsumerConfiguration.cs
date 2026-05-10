using DotPulsar;
using Messaging.Abstracts;

namespace Messaging.Pulsar.Configurations;

public interface IPulsarConsumerConfiguration
{
    MessageTopic Topic { get; }
    
    SubscriptionType SubscriptionType { get; }

    string SubscriptionName { get; }

    SubscriptionInitialPosition InitialPosition { get; }
    
    int Count { get; }
}
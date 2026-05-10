using DotPulsar;
using Messaging.Abstracts;

namespace Messaging.Pulsar.Configurations;

public record PulsarConsumerConfiguration : IPulsarConsumerConfiguration
{
    public MessageTopic Topic { get; init; }

    public SubscriptionType SubscriptionType { get; init; } = SubscriptionType.KeyShared;

    public string SubscriptionName { get; init; } = null!;

    public SubscriptionInitialPosition InitialPosition { get; init; } = SubscriptionInitialPosition.Earliest;

    public int Count { get; init; } = 1;
}
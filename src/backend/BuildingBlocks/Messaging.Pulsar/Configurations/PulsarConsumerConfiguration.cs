using DotPulsar;

namespace Messaging.Pulsar.Configurations;

public record PulsarConsumerConfiguration : IPulsarConsumerConfiguration
{
    public string Topic { get; init; } = null!;

    public SubscriptionType SubscriptionType { get; init; } = SubscriptionType.KeyShared;

    public string SubscriptionName { get; init; } = null!;

    public SubscriptionInitialPosition InitialPosition { get; init; } = SubscriptionInitialPosition.Earliest;

    public int Count { get; init; } = 1;
}
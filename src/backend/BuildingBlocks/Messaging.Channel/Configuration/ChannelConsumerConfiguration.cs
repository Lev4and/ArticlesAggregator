namespace Messaging.Channel.Configuration;

public record ChannelConsumerConfiguration : IChannelConsumerConfiguration
{
    public string Topic { get; init; } = null!;

    public int Count { get; init; } = 1;
}
using Messaging.Abstracts;

namespace Messaging.Channel.Configuration;

public record ChannelConsumerConfiguration : IChannelConsumerConfiguration
{
    public MessageTopic Topic { get; init; }

    public int Count { get; init; } = 1;
}
using Messaging.Abstracts;

namespace Messaging.Channel;

public record ChannelMessageContext : IMessageContext
{
    public string MessageId { get; init; } = null!;

    public IMessage Data { get; init; } = null!;
    
    public DateTime PublishedAt { get; init; }
}
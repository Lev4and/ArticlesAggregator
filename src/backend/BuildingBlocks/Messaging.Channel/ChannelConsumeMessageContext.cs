using Messaging.Abstracts;

namespace Messaging.Channel;

public record ChannelConsumeMessageContext : IConsumeMessageContext
{
    public string MessageId { get; init; } = null!;

    public IMessage Data { get; init; } = null!;
    
    public DateTime PublishedAt { get; init; }
    
    public DateTime ConsumedAt { get; init; }
}
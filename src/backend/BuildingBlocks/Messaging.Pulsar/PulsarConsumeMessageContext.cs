using Messaging.Abstracts;

namespace Messaging.Pulsar;

public record PulsarConsumeMessageContext : IConsumeMessageContext
{
    public string MessageId { get; init; } = null!;

    public IMessage Data { get; init; } = null!;
    
    public DateTime PublishedAt { get; init; }
    
    public DateTime ConsumedAt { get; init; }
}
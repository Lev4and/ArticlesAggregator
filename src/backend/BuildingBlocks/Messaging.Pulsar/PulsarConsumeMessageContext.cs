using Messaging.Abstracts;

namespace Messaging.Pulsar;

public record PulsarConsumeMessageContext(string MessageId, IMessage Data, DateTime PublishedAt, DateTime ConsumedAt) 
    : IConsumeMessageContext;
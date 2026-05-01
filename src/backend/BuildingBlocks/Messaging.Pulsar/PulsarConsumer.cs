using System.Runtime.CompilerServices;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Messaging.Abstracts;
using Messaging.Pulsar.Constants;
using Newtonsoft.Json;
using IMessage = Messaging.Abstracts.IMessage;

namespace Messaging.Pulsar;

public class PulsarConsumer : IMessageConsumer
{
    private readonly IConsumer<string> _consumer;
    
    public PulsarConsumer(IPulsarClient client)
    {
        _consumer = client.NewConsumer(Schema.String).Create();
    }
    
    public async IAsyncEnumerable<IConsumeMessageContext> ReceiveAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var consumedMessage in _consumer.Messages(ct))
        {
            if (!consumedMessage.Properties.TryGetValue(PulsarMessagePropertyConstants.Type, out var type))
            {
                await _consumer.Acknowledge(consumedMessage.MessageId, ct);
                    
                continue;
            }

            var messageType = Type.GetType(type);
            if (messageType is null)
            {
                await _consumer.Acknowledge(consumedMessage.MessageId, ct);
                    
                continue;
            }

            var message = (IMessage)JsonConvert.DeserializeObject(consumedMessage.Value(), messageType)!;
                    
            yield return new PulsarConsumeMessageContext
            {
                MessageId   = consumedMessage.MessageId.ToString(),
                Data        = message,
                PublishedAt = consumedMessage.PublishTimeAsDateTime,
                ConsumedAt  = DateTime.UtcNow
            };
        }
    }

    public async Task AcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default)
    {
        if (MessageId.TryParse(context.MessageId, out var messageId))
        {
            await _consumer.Acknowledge(messageId, ct);
        } 
    }

    public async Task NegativeAcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default)
    {
        if (MessageId.TryParse(context.MessageId, out var messageId))
        {
            await _consumer.RedeliverUnacknowledgedMessages([messageId], ct);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _consumer.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}
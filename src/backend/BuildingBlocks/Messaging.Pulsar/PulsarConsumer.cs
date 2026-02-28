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
        var consumerBuilder = client.NewConsumer(Schema.String);
        
        _consumer = consumerBuilder.Create();
    }
    
    public async IAsyncEnumerable<IConsumeMessageContext> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await foreach (var consumedMessage in _consumer.Messages(cancellationToken))
            {
                var messageType = consumedMessage.Properties[PulsarMessagePropertyConstants.Type];
                var message = (IMessage)JsonConvert.DeserializeObject(consumedMessage.Value(), Type.GetType(messageType)!)!;
                
                yield return new PulsarConsumeMessageContext(consumedMessage.MessageId.ToString(), message, 
                    consumedMessage.PublishTimeAsDateTime, DateTime.UtcNow);
            }
        }
    }

    public async Task AcknowledgeAsync(IConsumeMessageContext context, 
        CancellationToken cancellationToken = default)
    {
        if (MessageId.TryParse(context.MessageId, out var messageId))
        {
            await _consumer.Acknowledge(messageId, cancellationToken);
        } 
    }

    public async Task NegativeAcknowledgeAsync(IConsumeMessageContext context, 
        CancellationToken cancellationToken = default)
    {
        if (MessageId.TryParse(context.MessageId, out var messageId))
        {
            await _consumer.RedeliverUnacknowledgedMessages([messageId], cancellationToken);
        }
    }
}
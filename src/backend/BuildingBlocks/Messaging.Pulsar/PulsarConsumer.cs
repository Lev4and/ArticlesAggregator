using System.Runtime.CompilerServices;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Messaging.Abstracts;
using Messaging.Messages;
using Messaging.Pulsar.Constants;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Observability.Abstracts;
using IMessage = Messaging.Abstracts.IMessage;

namespace Messaging.Pulsar;

public class PulsarConsumer : IMessageConsumer
{
    private readonly ITracer<PulsarConsumer> _tracer;
    private readonly ILogger<PulsarConsumer> _logger;
    private readonly IConsumer<string> _consumer;
    
    public string Id { get; } = Guid.NewGuid().ToString();
    
    public PulsarConsumer(
        ITracer<PulsarConsumer> tracer,
        ILogger<PulsarConsumer> logger,
        IConsumer<string> consumer)
    {
        _tracer = tracer;
        _logger = logger;
        _consumer = consumer;
    }
    
    public async IAsyncEnumerable<IConsumeMessageContext> ReceiveAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        _logger.LogInformation("Channel consumer started ConsumerId: {PulsarConsumerId}", Id);
        
        await foreach (var consumedMessage in _consumer.Messages(ct))
        {
            _logger.LogInformation(
                "Received message Id: {MessageContextId} ConsumerId: {PulsarConsumerId}", 
                    consumedMessage.MessageId, Id);
            
            if (!consumedMessage.Properties.TryGetValue(PulsarMessagePropertyConstants.Type, out var type))
            {
                await _consumer.Acknowledge(consumedMessage.MessageId, ct);
                
                _logger.LogWarning("Received message haven`t type property Id: {MessageContextId}",
                    consumedMessage.MessageId);
                    
                continue;
            }

            var messageType = MessageTypeResolver.Resolve(type);
            if (messageType is null)
            {
                await _consumer.Acknowledge(consumedMessage.MessageId, ct);
                
                _logger.LogWarning(
                    "Message type not found in assembly Id: {MessageContextId} Type: {MessageType}",
                        consumedMessage.MessageId, type);
                    
                continue;
            }

            IMessage? message;

            try
            {
                message = (IMessage)JsonConvert.DeserializeObject(consumedMessage.Value(), messageType)!;
            }
            catch (Exception exception)
            {
                await _consumer.Acknowledge(consumedMessage.MessageId, ct);
                
                _logger.LogError(exception, 
                    "Deserialize message failed Id: {MessageContextId}",
                        consumedMessage.MessageId);
                
                continue;
            }

            yield return new PulsarConsumeMessageContext
            {
                MessageId   = consumedMessage.MessageId.ToString(),
                Data        = message,
                PublishedAt = consumedMessage.PublishTimeAsDateTime,
                ConsumedAt  = DateTime.UtcNow
            };
        }
        
        _logger.LogInformation("Channel consumer finished ConsumerId: {PulsarConsumerId}", Id);
    }

    public async Task AcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Acknowledge message (Pulsar)");
        
        _logger.LogInformation(
            "Acknowledge message Id: {MessageContextId} MessageId: {MessageId} ConsumerId: {PulsarConsumerId}", 
                context.MessageId, context.Data.Id, Id);
        
        if (!MessageId.TryParse(context.MessageId, out var messageId))
        {
            _logger.LogWarning(
                "Message has invalid id format Id: {MessageContextId} MessageId: {MessageId}", 
                    context.MessageId, context.Data.Id);
            
            return;
        }
        
        await _consumer.Acknowledge(messageId, ct);
    }

    public async Task NegativeAcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Negative acknowledge message (Pulsar)");
        
        _logger.LogInformation(
            "Negative acknowledge message Id: {MessageContextId} MessageId: {MessageId} ConsumerId: {PulsarConsumerId}", 
                context.MessageId, context.Data.Id, Id);
        
        if (!MessageId.TryParse(context.MessageId, out var messageId))
        {
            _logger.LogWarning(
                "Message has invalid id format Id: {MessageContextId} MessageId: {MessageId}", 
                    context.MessageId, context.Data.Id);
            
            return;
        }
        
        await _consumer.RedeliverUnacknowledgedMessages([messageId], ct);
    }

    public async ValueTask DisposeAsync()
    {
        await _consumer.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}
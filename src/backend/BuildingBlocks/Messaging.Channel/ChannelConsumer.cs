using System.Runtime.CompilerServices;
using Messaging.Abstracts;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace Messaging.Channel;

public class ChannelConsumer : IMessageConsumer
{
    private readonly ITracer<ChannelConsumer> _tracer;
    private readonly ILogger<ChannelConsumer> _logger;
    private readonly ChannelQueue             _queue;
    
    public string Id { get; } = Guid.NewGuid().ToString();

    public ChannelConsumer(
        ITracer<ChannelConsumer> tracer,
        ILogger<ChannelConsumer> logger,
        ChannelQueue             queue)
    {
        _tracer = tracer;
        _logger = logger;
        _queue  = queue;
    }
    
    public async IAsyncEnumerable<IConsumeMessageContext> ReceiveAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        _logger.LogInformation("Channel consumer started ConsumerId: {ChannelConsumerId}", Id);
        
        while (await _queue.Reader.WaitToReadAsync(ct))
        {
            var messageContext = await _queue.Reader.ReadAsync(ct);

            _logger.LogInformation(
                "Received message Id: {MessageContextId} MessageId: {MessageId} ConsumerId: {ChannelConsumerId}",
                    messageContext.MessageId, messageContext.Data.Id, Id);
                
            yield return new ChannelConsumeMessageContext
            {
                MessageId   = messageContext.MessageId,
                Data        = messageContext.Data,
                PublishedAt = messageContext.PublishedAt,
                ConsumedAt  = DateTime.UtcNow
            };
        }
        
        _logger.LogInformation("Channel consumer finished ConsumerId: {ChannelConsumerId}", Id);
    }

    public async Task AcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Acknowledge message (Channel)");

        _logger.LogInformation(
            "Acknowledge message Id: {MessageContextId} MessageId: {MessageId} ConsumerId: {ChannelConsumerId}", 
                context.MessageId, context.Data.Id, Id);
        
        await Task.CompletedTask;
    }

    public async Task NegativeAcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Negative acknowledge message (Channel)");
        
        _logger.LogInformation(
            "Negative acknowledge message Id: {MessageContextId} MessageId: {MessageId} ConsumerId: {ChannelConsumerId}", 
                context.MessageId, context.Data.Id, Id);
        
        await _queue.Writer.WriteAsync(context, ct);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
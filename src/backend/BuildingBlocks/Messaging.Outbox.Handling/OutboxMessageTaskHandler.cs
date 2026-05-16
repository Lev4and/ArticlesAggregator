using System.Text.Json;
using Messaging.Abstracts;
using Messaging.Messages;
using Messaging.Outbox.Abstracts;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using Result;
using StoredTasks.Abstracts;

namespace Messaging.Outbox.Handling;

public class OutboxMessageTaskHandler : IStoredTaskHandler<OutboxMessage>
{
    private readonly ITracer<OutboxMessageTaskHandler> _tracer;
    private readonly ILogger<OutboxMessageTaskHandler> _logger;
    private readonly IDistributedMessageProducer _messageProducer; 
    
    public OutboxMessageTaskHandler(
        ITracer<OutboxMessageTaskHandler> tracer, 
        ILogger<OutboxMessageTaskHandler> logger, 
        IDistributedMessageProducer messageProducer)
    {
        _tracer = tracer;
        _logger = logger;
        _messageProducer = messageProducer;
    }
    
    public async Task<AppResult> HandleAsync(OutboxMessage storedTask, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Outbox message task handle");
        
        _logger.LogInformation("Outbox message task handle");

        var messageType = MessageTypeResolver.Resolve(storedTask.Type);
        if (messageType is null)
        {
            _logger.LogError("Message type not found in assembly Type: {MessageType}", storedTask.Type);

            return AppResult.Success();
        }
        
        IMessage? message;

        try
        {
            message = (IMessage)storedTask.Content.Deserialize(messageType)!;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Deserialize message failed Type: {MessageType}", storedTask.Type);

            return AppResult.Success();
        }
        finally
        {
            storedTask.Content.Dispose();
        }
        
        await _messageProducer.PublishAsync(message, ct);
        
        return AppResult.Success();
    }
}
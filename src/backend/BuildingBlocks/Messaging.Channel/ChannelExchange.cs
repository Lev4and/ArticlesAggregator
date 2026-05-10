using System.Reflection;
using Messaging.Abstracts;
using Messaging.Abstracts.Attributes;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace Messaging.Channel;

public class ChannelExchange
{
    private readonly ITracer<ChannelExchange> _tracer;
    private readonly ILogger<ChannelExchange> _logger;
    private readonly ChannelQueueDictionary _queueDictionary;
    
    public ChannelExchange(
        ITracer<ChannelExchange> tracer,
        ILogger<ChannelExchange> logger,
        ChannelQueueDictionary queueDictionary)
    {
        _tracer = tracer;
        _logger = logger;
        _queueDictionary = queueDictionary;
    }
    
    public async Task PublishAsync(IMessage message, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Exchange message to channel queue");
        
        _logger.LogInformation("Exchange message to channel queue Id: {MessageId}", message.Id);
        
        var messageType = message.GetType();
        
        var messageTopicAttribute = message.GetType().GetCustomAttribute<MessageTopicAttribute>();
        if (messageTopicAttribute is null)
        {
            
            _logger.LogWarning("Message topic attribute not found Id: {MessageId} Type: {MessageType}", 
                message.Id, messageType);
            
            return;
        }
        
        var queue = _queueDictionary.GetOrAdd(messageTopicAttribute.Topic, new ChannelQueue());

        var messageContext = new ChannelMessageContext
        {
            MessageId   = Guid.NewGuid().ToString(),
            Data        = message,
            PublishedAt = DateTime.UtcNow
        };
        
        await queue.Writer.WriteAsync(messageContext, ct);
        
        _logger.LogInformation("Exchange message to channel queue completed Id: {MessageId}", message.Id);
    }
}
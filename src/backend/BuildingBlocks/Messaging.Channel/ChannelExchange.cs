using System.Reflection;
using Messaging.Abstracts;
using Messaging.Abstracts.Attributes;
using Microsoft.Extensions.Logging;

namespace Messaging.Channel;

public class ChannelExchange
{
    private readonly ILogger _logger;
    private readonly ChannelQueueDictionary _queueDictionary;
    
    public ChannelExchange(ILogger<ChannelExchange> logger, ChannelQueueDictionary queueDictionary)
    {
        _logger = logger;
        _queueDictionary = queueDictionary;
    }
    
    public async Task PublishAsync(IMessage message, CancellationToken cancellationToken = default)
    {
        var messageAttribute = message.GetType().GetCustomAttribute<MessageTopicAttribute>();
        if (messageAttribute is null)
        {
            _logger.LogWarning("Channel message attribute not found on type {MessageType}", message.GetType());
            
            return;
        }
        
        var queue = _queueDictionary.GetOrAdd(messageAttribute.Topic, new ChannelQueue());

        _logger.LogInformation("Redirect message Id: {MessageId} Type: {MessageType} to queue", message.Id, 
            message.GetType());
        
        var messageContext = new ChannelMessageContext(Guid.NewGuid().ToString(), message, DateTime.UtcNow);
        
        await queue.Writer.WriteAsync(messageContext, cancellationToken);

        _logger.LogInformation("Redirect message Id: {MessageId} Type: {MessageType} to queue completed", 
            message.Id, message.GetType());
    }
}
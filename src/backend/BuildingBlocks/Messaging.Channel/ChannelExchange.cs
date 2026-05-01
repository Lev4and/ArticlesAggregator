using System.Reflection;
using Messaging.Abstracts;
using Messaging.Abstracts.Attributes;

namespace Messaging.Channel;

public class ChannelExchange
{
    private readonly ChannelQueueDictionary _queueDictionary;
    
    public ChannelExchange(ChannelQueueDictionary queueDictionary)
    {
        _queueDictionary = queueDictionary;
    }
    
    public async Task PublishAsync(IMessage message, CancellationToken ct = default)
    {
        var messageTopicAttribute = message.GetType().GetCustomAttribute<MessageTopicAttribute>();
        if (messageTopicAttribute is null)
        {
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
    }
}
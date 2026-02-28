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
    
    public async Task PublishAsync(IMessage message, CancellationToken cancellationToken = default)
    {
        var messageAttribute = message.GetType().GetCustomAttribute<MessageTopicAttribute>();
        if (messageAttribute is null)
        {
            return;
        }
        
        var queue = _queueDictionary.GetOrAdd(messageAttribute.Topic, new ChannelQueue());

        var messageContext = new ChannelMessageContext(Guid.NewGuid().ToString(), message, DateTime.UtcNow);
        
        await queue.Writer.WriteAsync(messageContext, cancellationToken);
    }
}
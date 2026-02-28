using Messaging.Abstracts;

namespace Messaging.Channel;

public record ChannelConsumeMessageContext : IConsumeMessageContext
{
    public string MessageId { get; }
    
    public IMessage Data { get; }
    
    public DateTime PublishedAt { get; }
    
    public DateTime ConsumedAt { get; }

    public ChannelConsumeMessageContext(IMessageContext messageContext, DateTime consumedAt) : 
        this(messageContext.MessageId, messageContext.Data, consumedAt, consumedAt)
    {
        
    }

    public ChannelConsumeMessageContext(string messageId, IMessage data, DateTime publishedAt, DateTime consumedAt)
    {
        MessageId = messageId;
        Data = data;
        PublishedAt = publishedAt;
        ConsumedAt = consumedAt;
    }
}
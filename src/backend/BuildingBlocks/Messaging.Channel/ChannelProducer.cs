using Messaging.Abstracts;

namespace Messaging.Channel;

public class ChannelProducer : IMessageProducer
{
    private readonly ChannelExchange _exchange;
    
    public ChannelProducer(ChannelExchange exchange)
    {
        _exchange = exchange;
    }
    
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        await _exchange.PublishAsync(message, cancellationToken);
    }
}
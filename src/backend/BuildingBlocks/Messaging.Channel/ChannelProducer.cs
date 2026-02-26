using Messaging.Abstracts;
using Microsoft.Extensions.Logging;

namespace Messaging.Channel;

public class ChannelProducer : IMessageProducer
{
    private readonly ILogger _logger;
    private readonly ChannelExchange _exchange;
    
    public ChannelProducer(ILogger<ChannelProducer> logger, ChannelExchange exchange)
    {
        _logger = logger;
        _exchange = exchange;
    }
    
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        _logger.LogInformation("Publishing message Id: {MessageId} Body: {@MessageBody}", message.Id, message);
        
        await _exchange.PublishAsync(message, cancellationToken);
        
        _logger.LogInformation("Published message Id: {MessageId} Body: {@MessageBody}", message.Id, message);
    }
}
using Messaging.Abstracts;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace Messaging.Channel;

public class ChannelProducer : IMessageProducer
{
    private readonly ITracer<ChannelProducer> _tracer;
    private readonly ILogger<ChannelProducer> _logger;
    private readonly ChannelExchange _exchange;
    
    public ChannelProducer(
        ITracer<ChannelProducer> tracer,
        ILogger<ChannelProducer> logger,
        ChannelExchange exchange)
    {
        _tracer = tracer;
        _logger = logger;
        _exchange = exchange;
    }
    
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken ct = default) 
        where TMessage : IMessage
    {
        using var operation = _tracer.StartOperation("Publish message to channel");
        
        _logger.LogInformation("Publish message to channel Id: {MessageId}", message.Id);
        
        await _exchange.PublishAsync(message, ct);
        
        _logger.LogInformation("Publish message to channel completed Id: {MessageId}", message.Id);
    }

    public async ValueTask DisposeAsync()
    {
        // TODO release managed resources here
    }
}
using Messaging.Abstracts;
using Messaging.Channel.Configuration;
using Messaging.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace Messaging.Channel;

public class ChannelConsumerWorker : ConsumerWorker
{
    private readonly ChannelQueueDictionary        _queueDictionary;
    private readonly IChannelConsumerConfiguration _configuration;

    public ChannelConsumerWorker(
        ITracer<ConsumerWorker>       tracer, 
        ILogger<ConsumerWorker>       logger, 
        IServiceProvider              serviceProvider, 
        ChannelQueueDictionary        queueDictionary, 
        IChannelConsumerConfiguration configuration) : 
        base(tracer, logger, serviceProvider)
    {
        _queueDictionary = queueDictionary;
        _configuration   = configuration;
    }

    public virtual async Task RunAsync(CancellationToken ct = default)
    {
        Logger.LogInformation("Channel consumer worker started");
        
        var consumerTasks = Enumerable.Range(1, _configuration.Count)
            .Select(_ =>
            {
                var tracer = ServiceProvider.GetRequiredService<ITracer<ChannelConsumer>>();
                var logger = ServiceProvider.GetRequiredService<ILogger<ChannelConsumer>>();
                
                var queue  = _queueDictionary.GetOrAdd(_configuration.Topic, new ChannelQueue());
                
                return new ChannelConsumer(tracer, logger, queue);
            })
            .Select(async consumer =>
            {
                await RunConsumerAsync(consumer, ct);
            });
        
        await Task.WhenAll(consumerTasks);
        
        Logger.LogInformation("Channel consumer worker finished");
    }

    protected override async Task RunConsumerAsync(IMessageConsumer consumer, CancellationToken ct = default)
    {
        Logger.LogInformation(
            "Channel consumer started Id: {ChannelConsumerId} Topic: {MessageTopic}", 
                consumer.Id, _configuration.Topic);
        
        var loggingContext = new Dictionary<string, object>
        {
            { "ChannelConsumerId", consumer.Id },
            { "MessageTopic",      _configuration.Topic },
        };
        
        using var consumerLoggingScope = Logger.BeginScope(loggingContext);
        
        await base.RunConsumerAsync(consumer, ct);

        Logger.LogInformation(
            "Channel consumer finished Id: {ChannelConsumerId} Topic: {MessageTopic}",
                consumer.Id, _configuration.Topic);
    }
}

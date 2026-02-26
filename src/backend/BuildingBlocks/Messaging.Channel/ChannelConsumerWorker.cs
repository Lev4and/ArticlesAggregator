using Messaging.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging.Channel;

public abstract class ChannelConsumerWorker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    protected abstract string Topic { get; }

    protected virtual int ConsumerCount => 1;

    protected ChannelConsumerWorker(ILogger<ChannelConsumerWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope();

        var queueDict = scope.ServiceProvider.GetRequiredService<ChannelQueueDictionary>();
        var consumers = Enumerable.Range(1, ConsumerCount)
            .Select(_ =>
            {
                var queue = queueDict.GetOrAdd(Topic, new ChannelQueue());
                
                return new ChannelConsumer(queue);
            })
            .Select(async consumer =>
            {
                await foreach (var message in consumer.ReceiveAsync(stoppingToken))
                {
                    try
                    {
                        _logger.LogInformation("Received message Id: {MessageId} Body: {@MessageBody}", message.Id, message);
                        
                        var messageHandlerType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());
                        
                        var messageHandler = scope.ServiceProvider.GetService(messageHandlerType);
                        if (messageHandler is null)
                        {
                            _logger.LogWarning("Not found message handler for {MessageType}", message.GetType());
                            
                            continue;
                        }
                        
                        await ((IMessageHandler)messageHandler).HandleAsync(message, stoppingToken);

                        _logger.LogInformation("Message processed Id: {MessageId} Body: {@MessageBody}", message.Id, message);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception,
                            "An error occurred while processing message Id: {MessageId} Body: {@MessageBody}",
                                message.Id, message);
                    }
                }
            })
            .ToArray();

        await Task.WhenAll(consumers);
    }
}

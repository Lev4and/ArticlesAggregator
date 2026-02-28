using Messaging.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging.Channel;

public abstract class ChannelConsumerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    protected abstract string Topic { get; }

    protected virtual int ConsumerCount => 1;

    protected ChannelConsumerWorker(IServiceProvider serviceProvider)
    {
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
                await foreach (var messageConsumedContext in consumer.ReceiveAsync(stoppingToken))
                {
                    var message = messageConsumedContext.Data;
                    
                    try
                    {
                        var messageHandlerType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());
                        
                        var messageHandler = scope.ServiceProvider.GetService(messageHandlerType);
                        if (messageHandler is null)
                        {
                            continue;
                        }
                        
                        var messageHandleResult = await ((IMessageHandler)messageHandler)
                            .HandleAsync(message, stoppingToken);
                        if (messageHandleResult.IsSuccess)
                        {
                            await consumer.AcknowledgeAsync(messageConsumedContext, stoppingToken);
                        }
                        else
                        {
                            await consumer.NegativeAcknowledgeAsync(messageConsumedContext, stoppingToken);
                        }
                    }
                    catch
                    {
                        await consumer.NegativeAcknowledgeAsync(messageConsumedContext, stoppingToken);
                    }
                }
            })
            .ToArray();

        await Task.WhenAll(consumers);
    }
}

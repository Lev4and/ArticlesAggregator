using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Messaging.Abstracts;
using Messaging.Hosting;
using Messaging.Pulsar.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace Messaging.Pulsar;

public class PulsarConsumerWorker : ConsumerWorker
{
    private readonly IPulsarClient                _client;
    private readonly IPulsarConsumerConfiguration _configuration;
    private readonly IPulsarTopicConfiguration    _topicConfiguration;

    public PulsarConsumerWorker(
        ITracer<ConsumerWorker>      tracer, 
        ILogger<ConsumerWorker>      logger, 
        IServiceProvider             serviceProvider, 
        IPulsarClient                client, 
        IPulsarConsumerConfiguration configuration, 
        IPulsarTopicConfiguration    topicConfiguration) : 
        base(tracer, logger, serviceProvider)
    {
        _client             = client;
        _configuration      = configuration;
        _topicConfiguration = topicConfiguration;
    }

    public virtual async Task RunAsync(CancellationToken ct = default)
    {
        Logger.LogInformation("Pulsar consumer worker started");
        
        var consumerTasks = Enumerable.Range(1, _configuration.Count)
            .Select(_ =>
            {
                var tracer = ServiceProvider.GetRequiredService<ITracer<PulsarConsumer>>();
                var logger = ServiceProvider.GetRequiredService<ILogger<PulsarConsumer>>();

                var consumerBuilder = _client.NewConsumer(Schema.String)
                    .Topic(_topicConfiguration.Map(_configuration.Topic))
                    .SubscriptionType(_configuration.SubscriptionType)
                    .SubscriptionName(_configuration.SubscriptionName)
                    .InitialPosition(_configuration.InitialPosition);
                
                var consumer = consumerBuilder.Create();
                
                return new PulsarConsumer(tracer, logger, consumer);
            })
            .Select(async consumer =>
            {
                await RunConsumerAsync(consumer, ct);
            });
        
        await Task.WhenAll(consumerTasks);
        
        Logger.LogInformation("Pulsar consumer worker finished");
    }

    protected override async Task RunConsumerAsync(IMessageConsumer consumer, CancellationToken ct = default)
    {
        Logger.LogInformation(
            "Pulsar consumer started Id: {PulsarConsumerId} Topic: {MessageTopic}", 
                consumer.Id, _configuration.Topic);
        
        var loggingContext = new Dictionary<string, object>
        {
            { "PulsarConsumerId", consumer.Id },
            { "MessageTopic",     _configuration.Topic },
        };
        
        using var consumerLoggingScope = Logger.BeginScope(loggingContext);
        
        await base.RunConsumerAsync(consumer, ct);
        
        Logger.LogInformation(
            "Pulsar consumer finished Id: {PulsarConsumerId} Topic: {MessageTopic}",
                consumer.Id, _configuration.Topic);
    }
}
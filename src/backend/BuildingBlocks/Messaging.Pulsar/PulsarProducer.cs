using System.Reflection;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Messaging.Abstracts;
using Messaging.Abstracts.Attributes;
using Messaging.Pulsar.Configurations;
using Messaging.Pulsar.Constants;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Observability.Abstracts;
using IMessage = Messaging.Abstracts.IMessage;

namespace Messaging.Pulsar;

public class PulsarProducer : IDistributedMessageProducer
{
    private readonly ITracer<PulsarProducer>   _tracer;
    private readonly ILogger<PulsarProducer>   _logger;
    private readonly IPulsarClient             _client;
    private readonly IPulsarTopicConfiguration _topicConfiguration;
    private readonly PulsarProducerDictionary  _producerDictionary;
    
    public PulsarProducer(
        ITracer<PulsarProducer>   tracer,
        ILogger<PulsarProducer>   logger,
        IPulsarClient             client, 
        IPulsarTopicConfiguration topicConfiguration,
        PulsarProducerDictionary  producerDictionary)
    {
        _tracer             = tracer;
        _logger             = logger;
        _client             = client;
        _topicConfiguration = topicConfiguration;
        _producerDictionary = producerDictionary;
    }
    
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken ct = default) 
        where TMessage : IMessage
    {
        using var operation = _tracer.StartOperation("Publish message to Pulsar");
        
        _logger.LogInformation("Publish message to Pulsar Id: {MessageId}", message.Id);
        
        var messageTypeAttribute = message.GetType().GetCustomAttribute<MessageTypeAttribute>();
        if (messageTypeAttribute is null)
        {
            _logger.LogWarning("Message type attribute not found Id: {MessageId} Type: {MessageType}", 
                message.Id, message.GetType());
            
            return;
        }
        
        var messageTopicAttribute = message.GetType().GetCustomAttribute<MessageTopicAttribute>();
        if (messageTopicAttribute is null)
        {
            _logger.LogWarning("Message topic attribute not found Id: {MessageId} Type: {MessageType}", 
                message.Id, message.GetType());
            
            return;
        }

        await using var producer = _client.NewProducer(Schema.String)
            .Topic(_topicConfiguration.Map(messageTopicAttribute.Topic))
            .Create();

        var messageBody = JsonConvert.SerializeObject(message);
        var messageBuilder   = producer.NewMessage()
            .Property(PulsarMessagePropertyConstants.Type, messageTypeAttribute.Type);
        
        await messageBuilder.Send(messageBody, ct);
        
        _logger.LogInformation("Publish message to Pulsar completed Id: {MessageId}", message.Id);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}
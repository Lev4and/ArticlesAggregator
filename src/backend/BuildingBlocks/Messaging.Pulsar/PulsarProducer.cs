using System.Reflection;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Messaging.Abstracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IMessage = Messaging.Abstracts.IMessage;

namespace Messaging.Pulsar;

public class PulsarProducer : IDistributedMessageProducer
{
    private readonly ILogger _logger;
    private readonly IPulsarClient _client;
    private readonly PulsarProducerDictionary _producerDictionary;
    
    public PulsarProducer(
        ILogger<PulsarProducer> logger, 
        IPulsarClient client, 
        PulsarProducerDictionary producerDictionary)
    {
        _logger = logger;
        _client = client;
        _producerDictionary = producerDictionary;
    }
    
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        var messageAttribute = message.GetType().GetCustomAttribute<PulsarMessageAttribute>();
        if (messageAttribute is null)
        {
            _logger.LogWarning("Pulsar message attribute not found on type {MessageType}", message.GetType());
            
            return;
        }

        if (!_producerDictionary.ContainsKey(messageAttribute.Topic))
        {
            var producerBuilder = _client.NewProducer(Schema.String).Topic(messageAttribute.Topic);
            
            _producerDictionary.TryAdd(messageAttribute.Topic, producerBuilder.Create());
        }

        var producer = _producerDictionary[messageAttribute.Topic];
        
        var messageType = typeof(TMessage).Name;
        var messageBody = JsonConvert.SerializeObject(message);

        var messageBuilder = producer.NewMessage();
        
        messageBuilder.Property("type", messageType);

        _logger.LogInformation("Publishing message Id: {MessageId} Body: {@MessageBody}", message.Id, message);

        await messageBuilder.Send(messageBody, cancellationToken);
        
        _logger.LogInformation("Published message Id: {MessageId} Body: {@MessageBody}", message.Id, message);
    }
}
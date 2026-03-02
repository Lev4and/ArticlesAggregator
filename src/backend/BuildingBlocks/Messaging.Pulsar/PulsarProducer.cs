using System.Reflection;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Messaging.Abstracts.Attributes;
using Messaging.Abstracts.Distributed;
using Messaging.Pulsar.Constants;
using Newtonsoft.Json;
using IMessage = Messaging.Abstracts.IMessage;

namespace Messaging.Pulsar;

public class PulsarProducer : IDistributedMessageProducer
{
    private readonly IPulsarClient _client;
    private readonly PulsarProducerDictionary _producerDictionary;
    
    public PulsarProducer(
        IPulsarClient client, 
        PulsarProducerDictionary producerDictionary)
    {
        _client = client;
        _producerDictionary = producerDictionary;
    }
    
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        var messageTypeAttribute = message.GetType().GetCustomAttribute<MessageTypeAttribute>();
        var messageType = messageTypeAttribute?.Type ?? typeof(TMessage).Name;
        
        var messageTopicAttribute = message.GetType().GetCustomAttribute<MessageTopicAttribute>();
        if (messageTopicAttribute is null)
        {
            return;
        }

        if (!_producerDictionary.ContainsKey(messageTopicAttribute.Topic))
        {
            var producerBuilder = _client.NewProducer(Schema.String).Topic(messageTopicAttribute.Topic);
            
            _producerDictionary.TryAdd(messageTopicAttribute.Topic, producerBuilder.Create());
        }

        var producer = _producerDictionary[messageTopicAttribute.Topic];
        
        var messageBody = JsonConvert.SerializeObject(message);

        var messageBuilder = producer.NewMessage();
        
        messageBuilder.Property(PulsarMessagePropertyConstants.Type, messageType);

        await messageBuilder.Send(messageBody, cancellationToken);
    }
}
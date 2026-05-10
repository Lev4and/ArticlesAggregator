using System.Collections.Concurrent;
using DotPulsar.Abstractions;
using Messaging.Abstracts;

namespace Messaging.Pulsar;

public class PulsarProducerDictionary : ConcurrentDictionary<MessageTopic, IProducer<string>> 
{
    
}
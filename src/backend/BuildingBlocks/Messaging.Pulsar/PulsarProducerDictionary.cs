using System.Collections.Concurrent;
using DotPulsar.Abstractions;

namespace Messaging.Pulsar;

public class PulsarProducerDictionary : ConcurrentDictionary<string, IProducer<string>> 
{
    
}
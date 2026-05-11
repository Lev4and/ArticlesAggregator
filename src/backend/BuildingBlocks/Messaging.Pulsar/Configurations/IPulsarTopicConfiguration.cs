using Messaging.Abstracts;

namespace Messaging.Pulsar.Configurations;

public interface IPulsarTopicConfiguration
{
    string ProxyServerEventsTopic { get; }
    
    string Map(MessageTopic messageTopic);
}
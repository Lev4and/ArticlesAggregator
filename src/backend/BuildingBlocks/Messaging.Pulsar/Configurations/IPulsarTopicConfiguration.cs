using Messaging.Abstracts;

namespace Messaging.Pulsar.Configurations;

public interface IPulsarTopicConfiguration
{
    string Map(MessageTopic messageTopic);
}
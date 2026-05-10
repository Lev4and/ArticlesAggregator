using Messaging.Abstracts;

namespace Messaging.Pulsar.Configurations;

public class PulsarTopicConfiguration : IPulsarTopicConfiguration
{
    public string Map(MessageTopic messageTopic)
    {
        return messageTopic switch
        {
            _ => throw new NotSupportedException(nameof(messageTopic))
        };
    }
}
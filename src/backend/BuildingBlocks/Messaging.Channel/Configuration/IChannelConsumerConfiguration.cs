using Messaging.Abstracts;

namespace Messaging.Channel.Configuration;

public interface IChannelConsumerConfiguration
{
    MessageTopic Topic { get; }
    
    int Count { get; }
}
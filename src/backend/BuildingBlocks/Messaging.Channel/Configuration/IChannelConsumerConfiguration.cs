namespace Messaging.Channel.Configuration;

public interface IChannelConsumerConfiguration
{
    string Topic { get; }
    
    int Count { get; }
}
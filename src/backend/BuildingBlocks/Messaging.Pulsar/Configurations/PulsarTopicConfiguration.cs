using Messaging.Abstracts;

namespace Messaging.Pulsar.Configurations;

public class PulsarTopicConfiguration : IPulsarTopicConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string PROXY_SERVER_EVENTS_TOPIC = nameof(PROXY_SERVER_EVENTS_TOPIC);
    
    // ReSharper restore InconsistentNaming

    public string ProxyServerEventsTopic => Environment.GetEnvironmentVariable(PROXY_SERVER_EVENTS_TOPIC) 
        ?? "persistent://public/proxy-server/proxy-server-events";

    public string Map(MessageTopic messageTopic)
    {
        return messageTopic switch
        {
            MessageTopic.ProxyServerEvents => ProxyServerEventsTopic,
            _ => throw new NotSupportedException(nameof(messageTopic))
        };
    }
}
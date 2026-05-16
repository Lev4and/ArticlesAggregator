using Messaging.Abstracts;

namespace Messaging.Pulsar.Configurations;

public class PulsarTopicConfiguration : IPulsarTopicConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string PROXY_SERVER_EVENTS_TOPIC              = nameof(PROXY_SERVER_EVENTS_TOPIC);
    private const string PROXY_SERVER_TEST_EVENTS_TOPIC         = nameof(PROXY_SERVER_TEST_EVENTS_TOPIC);
    private const string PROXY_SERVER_TEST_RESULT_EVENTS_TOPIC  = nameof(PROXY_SERVER_TEST_RESULT_EVENTS_TOPIC);
    
    // ReSharper restore InconsistentNaming

    public string ProxyServerEventsTopic => Environment.GetEnvironmentVariable(PROXY_SERVER_EVENTS_TOPIC) 
        ?? "persistent://public/proxy-server/proxy-server-events";

    public string ProxyServerTestEventsTopic => Environment.GetEnvironmentVariable(PROXY_SERVER_TEST_EVENTS_TOPIC) 
        ?? "persistent://public/proxy-server/proxy-server-test-events";
    
    public string ProxyServerTestResultEventsTopic => 
        Environment.GetEnvironmentVariable(PROXY_SERVER_TEST_RESULT_EVENTS_TOPIC) 
            ?? "persistent://public/proxy-server/proxy-server-test-result-events";

    public string Map(MessageTopic messageTopic)
    {
        return messageTopic switch
        {
            MessageTopic.ProxyServerEvents => ProxyServerEventsTopic,
            MessageTopic.ProxyServerTestEvents => ProxyServerTestEventsTopic,
            MessageTopic.ProxyServerTestResultEvents => ProxyServerTestResultEventsTopic,
            _ => throw new NotSupportedException(nameof(messageTopic))
        };
    }
}
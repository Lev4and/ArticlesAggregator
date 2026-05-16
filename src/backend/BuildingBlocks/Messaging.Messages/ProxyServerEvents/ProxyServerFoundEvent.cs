using Messaging.Abstracts;
using Messaging.Abstracts.Attributes;
using Messaging.Messages.ProxyServerEvents.Models;

namespace Messaging.Messages.ProxyServerEvents;

[MessageType("proxy-server-found-event")]
[MessageTopic(MessageTopic.ProxyServerEvents)]
public record ProxyServerFoundEvent : BaseMessage
{
    public Guid ProxyServerId { get; init; }

    public string NormalizedName { get; init; } = null!;
    
    public ProxyServerProtocol Protocol { get; init; }
    
    public string HostnameOrAddress { get; init; } = null!;
    
    public int Port { get; init; }

    public ProxyServerCredentials? Credentials { get; init; }
}
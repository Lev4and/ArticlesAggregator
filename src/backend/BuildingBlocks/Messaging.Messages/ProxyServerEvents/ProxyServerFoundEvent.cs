using Messaging.Abstracts;
using Messaging.Abstracts.Attributes;

namespace Messaging.Messages.ProxyServerEvents;

[MessageType("proxy-server-found-event")]
[MessageTopic(MessageTopic.ProxyServerEvents)]
public record ProxyServerFoundEvent : BaseMessage
{
    public Guid ProxyServerId { get; init; }
}
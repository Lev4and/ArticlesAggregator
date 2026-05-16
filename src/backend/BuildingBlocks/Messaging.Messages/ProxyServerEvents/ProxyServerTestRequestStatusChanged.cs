using Messaging.Abstracts;
using Messaging.Abstracts.Attributes;
using Messaging.Messages.ProxyServerEvents.Models;

namespace Messaging.Messages.ProxyServerEvents;

[MessageType("proxy-server-test-request-status-changed-event")]
[MessageTopic(MessageTopic.ProxyServerTestResultEvents)]
public record ProxyServerTestRequestStatusChanged : BaseMessage
{
    public Guid RequestId { get; init; }
    
    public ProxyServerTestRequestStatus Status { get; init; }
    
    public long? RequestTime { get; init; }
    
    public long? ResponseTime { get; init; }
    
    public string? ErrorMessage { get; init; }
}
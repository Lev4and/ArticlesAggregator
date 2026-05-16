using Messaging.Messages.ProxyServerEvents.Models;

namespace ProxyServerAggregator.Application.Dtos.ProxyServers;

public record ProxyServerTestRequestResult
{
    public Guid RequestId { get; init; }
    
    public ProxyServerTestRequestStatus Status { get; init; }
    
    public long? RequestTime { get; set; }
    
    public long? ResponseTime { get; set; }
    
    public string? ErrorMessage { get; set; }
}
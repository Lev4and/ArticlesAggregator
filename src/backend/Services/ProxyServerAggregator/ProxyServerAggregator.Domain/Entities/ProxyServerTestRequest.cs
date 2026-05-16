using Contracts;
using Messaging.Messages.ProxyServerEvents.Models;
using Primitives;

namespace ProxyServerAggregator.Domain.Entities;

public class ProxyServerTestRequest : EntityBase<Guid>, IHasEntityState, IHasTimestamps, IHasSoftDelete
{
    public Guid ProxyServerId { get; set; }

    public ProxyServerTestRequestStatus Status { get; set; } = ProxyServerTestRequestStatus.Created;
    
    public long? RequestTime { get; set; }
    
    public long? ResponseTime { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public EntityState EntityState { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    public ProxyServer? Server { get; set; }
}
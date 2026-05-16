using Contracts;
using Messaging.Messages.ProxyServerEvents.Models;
using Primitives;

namespace ProxyServerAggregator.Domain.Entities;

public class ProxyServer : EntityBase<Guid>, IHasEntityState, IHasTimestamps, IHasSoftDelete
{
    public string NormalizedName { get; set; } = null!;
    
    public ProxyServerProtocol Protocol { get; set; }
    
    public string HostnameOrAddress { get; set; } = null!;
    
    public int Port { get; set; }

    public ProxyServerCredentials? Credentials { get; set; }
    
    public EntityState EntityState { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    public List<ProxyServerTestRequest>? TestRequests { get; init; }
}
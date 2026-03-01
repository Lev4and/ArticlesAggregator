using Contracts;
using Primitives;
using ProxyServerSearcher.Domain.Enums;
using ProxyServerSearcher.Domain.ValueObjects;

namespace ProxyServerSearcher.Domain.Entities;

public class ProxyServer : EntityBase<Guid>, IHasEntityState, IHasTimestamps, IHasSoftDelete
{
    public ProxyServerProtocol Protocol { get; set; }

    public string Host { get; set; } = null!;
    
    public int Port { get; set; }

    public ProxyServerCredentials? Credentials { get; set; }
    
    public EntityState EntityState { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }

    public ProxyServer Create(ProxyServerProtocol protocol, string host, int port, 
        ProxyServerCredentials? credentials = null)
    {
        var entity = new ProxyServer
        {
            Protocol = protocol,
            Host = host,
            Port = port,
            Credentials = credentials
        };
        
        return entity;
    }
}
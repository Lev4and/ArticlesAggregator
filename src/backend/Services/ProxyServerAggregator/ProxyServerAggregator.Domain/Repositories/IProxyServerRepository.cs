using Database.Abstracts;
using ProxyServerAggregator.Domain.Entities;

namespace ProxyServerAggregator.Domain.Repositories;

public interface IProxyServerRepository : IRepository<ProxyServer, Guid>
{
    
}
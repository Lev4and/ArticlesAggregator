using Database.Abstracts;
using ProxyServerAggregator.Domain.Dtos.ProxyServers;
using ProxyServerAggregator.Domain.Entities;

namespace ProxyServerAggregator.Domain.Repositories;

public interface IProxyServerRepository : IRepository<ProxyServer, Guid>
{
    Task<ProxyServerDto?> GetAsync(Guid id, CancellationToken ct = default);
}
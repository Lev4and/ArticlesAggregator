using ProxyServerAggregator.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerAggregator.Application.Abstracts.ProxyServers;

public interface IProxyServerCacheService
{
    Task SetAsync(ProxyServerDto proxyServer, CancellationToken ct = default);
    
    Task<ProxyServerDto?> GetAsync(Guid id, CancellationToken ct = default);
}
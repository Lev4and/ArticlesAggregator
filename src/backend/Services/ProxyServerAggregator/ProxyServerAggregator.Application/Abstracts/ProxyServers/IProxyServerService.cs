using ProxyServerAggregator.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerAggregator.Application.Abstracts.ProxyServers;

public interface IProxyServerService : IAsyncDisposable, IDisposable
{
    Task<AppResult> CreateAsync(ProxyServerDto model, CancellationToken ct = default);
    
    Task<AppResult<ProxyServerDto>> GetAsync(Guid id, CancellationToken ct = default); 
}
using ProxyServerAggregator.Application.Dtos.ProxyServers;
using Result;

namespace ProxyServerAggregator.Application.Abstracts.ProxyServers;

public interface IProxyServerService : IAsyncDisposable, IDisposable
{
    Task<AppResult> CreateAsync(ProxyServerDto model, CancellationToken ct = default);
}
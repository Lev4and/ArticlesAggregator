using ProxyServerSearcher.Application.Dtos.ProxyServers;
using Result;

namespace ProxyServerSearcher.Application.Abstracts.ProxyServers;

public interface IProxyServerService : IAsyncDisposable, IDisposable
{
    Task<AppResult> CreateBatchAsync(ProxyServerDto[] models, CancellationToken ct = default);
}
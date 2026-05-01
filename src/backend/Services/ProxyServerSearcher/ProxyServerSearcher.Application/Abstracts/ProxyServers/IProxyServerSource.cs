using ProxyServerSearcher.Application.Dtos.ProxyServers;

namespace ProxyServerSearcher.Application.Abstracts.ProxyServers;

public interface IProxyServerSource : IDisposable
{
    IAsyncEnumerable<IReadOnlyCollection<ProxyServerDto>> ProvideAsync(CancellationToken ct = default);
}
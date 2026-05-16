using ProxyServerAggregator.Application.Dtos.ProxyServers;
using Result;

namespace ProxyServerAggregator.Application.Abstracts.ProxyServers;

public interface IProxyServerTestRequestService
{
    Task<AppResult> UpdateAsync(ProxyServerTestRequestResult requestResult, CancellationToken ct = default);
}
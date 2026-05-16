using ProxyServerTester.Application.Dtos.ProxyServers;
using ProxyServerTester.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerTester.Application.Abstracts.ProxyServers;

public interface IProxyServerTester
{
    Task<AppResult<ProxyServerTestResult>> TestAsync(ProxyServerDto proxyServer, CancellationToken ct = default);
}
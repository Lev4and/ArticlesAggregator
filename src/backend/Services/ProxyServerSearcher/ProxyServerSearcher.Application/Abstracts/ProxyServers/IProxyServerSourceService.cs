using Result;

namespace ProxyServerSearcher.Application.Abstracts.ProxyServers;

public interface IProxyServerSourceService
{
    Task<AppResult<string[]>> GetListAsync(CancellationToken ct = default);
}
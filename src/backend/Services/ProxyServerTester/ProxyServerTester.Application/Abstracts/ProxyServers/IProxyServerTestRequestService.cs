using ProxyServerTester.Application.Dtos.ProxyServers;
using ProxyServerTester.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerTester.Application.Abstracts.ProxyServers;

public interface IProxyServerTestRequestService
{
    Task<AppResult> CreateAsync(ProxyServerTestRequestDto model, CancellationToken ct = default);
    
    Task<AppResult<ProxyServerTestRequestDto>> GetAsync(Guid id, CancellationToken ct = default);
    
    Task<AppResult> UpdateAsync(ProxyServerTestRequestResult requestResult, CancellationToken ct = default);
}
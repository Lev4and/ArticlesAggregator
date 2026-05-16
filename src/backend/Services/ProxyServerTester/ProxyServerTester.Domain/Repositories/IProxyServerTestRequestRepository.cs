using Database.Abstracts;
using ProxyServerTester.Domain.Dtos.ProxyServers;
using ProxyServerTester.Domain.Entities;

namespace ProxyServerTester.Domain.Repositories;

public interface IProxyServerTestRequestRepository : IRepository<ProxyServerTestRequest, Guid>
{
    Task<ProxyServerTestRequestDto?> GetAsync(Guid id, CancellationToken ct = default);
}
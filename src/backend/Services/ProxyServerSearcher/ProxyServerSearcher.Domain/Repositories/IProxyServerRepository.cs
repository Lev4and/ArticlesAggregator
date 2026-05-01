using Database.Abstracts;
using ProxyServerSearcher.Domain.Entities;

namespace ProxyServerSearcher.Domain.Repositories;

public interface IProxyServerRepository : IRepository<ProxyServer, Guid>
{
    Task<IReadOnlyCollection<string>> GetExistsAsync(string[] names, CancellationToken ct = default);
}
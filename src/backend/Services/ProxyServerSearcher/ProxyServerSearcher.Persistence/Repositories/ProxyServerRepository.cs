using Microsoft.EntityFrameworkCore;
using ProxyServerSearcher.Domain.Entities;
using ProxyServerSearcher.Domain.Repositories;

namespace ProxyServerSearcher.Persistence.Repositories;

public class ProxyServerRepository : AppDbContextRepository<ProxyServer, Guid>, IProxyServerRepository
{
    public ProxyServerRepository(AppDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<IReadOnlyCollection<string>> GetExistsAsync(string[] names, CancellationToken ct = default)
    {
        return await DbContext.Set<ProxyServer>().AsNoTracking()
            .Where(proxyServer => names.Contains(proxyServer.NormalizedName))
                .Select(proxyServer => proxyServer.NormalizedName)
                    .ToArrayAsync(ct);
    }
}
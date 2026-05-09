using Database.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Domain.Entities;
using ProxyServerSearcher.Domain.Repositories;

namespace ProxyServerSearcher.Persistence.Repositories;

public class ProxyServerRepository : AppDbContextRepository<ProxyServer, Guid>, IProxyServerRepository
{
    public ProxyServerRepository(
        ITracer<EntityFrameworkRepository<AppDbContext, ProxyServer, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<AppDbContext, ProxyServer, Guid>> logger, 
        AppDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }

    public async Task<IReadOnlyCollection<string>> GetExistsAsync(string[] names, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Get exists proxy servers from db");
        
        Logger.LogInformation("Get exists proxy servers from db");
        
        return await DbContext.Set<ProxyServer>().AsNoTracking()
            .Where(proxyServer => names.Contains(proxyServer.NormalizedName))
            .Select(proxyServer => proxyServer.NormalizedName)
            .ToArrayAsync(ct);
    }
}
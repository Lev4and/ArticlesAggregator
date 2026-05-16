using Database.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Domain.Dtos.ProxyServers;
using ProxyServerAggregator.Domain.Entities;
using ProxyServerAggregator.Domain.Repositories;

namespace ProxyServerAggregator.Persistence.Repositories;

public class ProxyServerRepository : AppDbContextRepository<ProxyServer, Guid>, IProxyServerRepository
{
    public ProxyServerRepository(
        ITracer<EntityFrameworkRepository<AppDbContext, ProxyServer, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<AppDbContext, ProxyServer, Guid>> logger, 
        AppDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }

    public async Task<ProxyServerDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Get proxy server");
        
        Logger.LogInformation("Get proxy server Id: {ProxyServerId}", id);
        
        return await DbContext.Set<ProxyServer>().AsNoTracking()
            .Select(proxyServer => new ProxyServerDto
            {
                Id                = proxyServer.Id,
                NormalizedName    = proxyServer.NormalizedName, 
                Protocol          = proxyServer.Protocol,
                HostnameOrAddress = proxyServer.HostnameOrAddress,
                Port              = proxyServer.Port,
                Credentials       = proxyServer.Credentials
            })
            .FirstOrDefaultAsync(proxyServer => proxyServer.Id == id, ct);
    }
}
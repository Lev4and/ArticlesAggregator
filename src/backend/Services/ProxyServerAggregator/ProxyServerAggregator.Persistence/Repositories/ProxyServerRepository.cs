using Database.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
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
}
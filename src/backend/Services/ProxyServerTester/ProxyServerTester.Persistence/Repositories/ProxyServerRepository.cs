using Database.EntityFramework.Repositories;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerTester.Domain.Entities;
using ProxyServerTester.Domain.Repositories;

namespace ProxyServerTester.Persistence.Repositories;

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
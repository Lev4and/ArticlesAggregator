using Database.EntityFramework.Repositories;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Domain.Entities;
using ProxyServerAggregator.Domain.Repositories;

namespace ProxyServerAggregator.Persistence.Repositories;

public class ProxyServerTestRequestRepository : AppDbContextRepository<ProxyServerTestRequest, Guid>, 
    IProxyServerTestRequestRepository
{
    public ProxyServerTestRequestRepository(
        ITracer<EntityFrameworkRepository<AppDbContext, ProxyServerTestRequest, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<AppDbContext, ProxyServerTestRequest, Guid>> logger, 
        AppDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }
}
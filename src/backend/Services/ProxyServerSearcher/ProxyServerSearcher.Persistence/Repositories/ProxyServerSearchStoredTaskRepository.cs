using Database.EntityFramework.Repositories;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Domain.Entities;
using StoredTasks.Database.EntityFramework.Repositories;

namespace ProxyServerSearcher.Persistence.Repositories;

public class ProxyServerSearchStoredTaskRepository : StoredTaskRepository<AppDbContext, ProxyServerSearchStoredTask>
{
    public ProxyServerSearchStoredTaskRepository(
        ITracer<EntityFrameworkRepository<AppDbContext, ProxyServerSearchStoredTask, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<AppDbContext, ProxyServerSearchStoredTask, Guid>> logger, 
        AppDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }
}
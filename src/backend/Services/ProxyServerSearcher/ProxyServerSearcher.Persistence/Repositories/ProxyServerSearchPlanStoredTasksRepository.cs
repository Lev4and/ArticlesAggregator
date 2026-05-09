using Database.EntityFramework.Repositories;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Domain.Entities;
using StoredTasks.Database.EntityFramework.Repositories;

namespace ProxyServerSearcher.Persistence.Repositories;

public class ProxyServerSearchPlanStoredTasksRepository : 
    StoredTaskRepository<AppDbContext, ProxyServerSearchPlanStoredTask>
{
    public ProxyServerSearchPlanStoredTasksRepository(
        ITracer<EntityFrameworkRepository<AppDbContext, ProxyServerSearchPlanStoredTask, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<AppDbContext, ProxyServerSearchPlanStoredTask, Guid>> logger, 
        AppDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }
}
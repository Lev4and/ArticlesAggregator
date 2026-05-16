using Database.EntityFramework.Repositories;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerTester.Domain.Entities;
using StoredTasks.Database.EntityFramework.Repositories;

namespace ProxyServerTester.Persistence.Repositories;

public class ProxyServerTestStoredTaskRepository : StoredTaskRepository<AppDbContext, ProxyServerTestStoredTask>
{
    public ProxyServerTestStoredTaskRepository(
        ITracer<EntityFrameworkRepository<AppDbContext, ProxyServerTestStoredTask, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<AppDbContext, ProxyServerTestStoredTask, Guid>> logger, 
        AppDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }
}
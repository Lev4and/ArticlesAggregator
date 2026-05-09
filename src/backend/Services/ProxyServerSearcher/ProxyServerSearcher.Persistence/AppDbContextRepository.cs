using Database.EntityFramework;
using Database.EntityFramework.Repositories;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using Primitives;

namespace ProxyServerSearcher.Persistence;

public abstract class AppDbContextRepository<TEntity, TKey> : EntityFrameworkRepository<AppDbContext, TEntity, TKey>
    where TEntity : EntityBase<TKey>
    where TKey : notnull
{
    protected AppDbContextRepository(
        ITracer<EntityFrameworkRepository<AppDbContext, TEntity, TKey>> tracer, 
        ILogger<EntityFrameworkRepository<AppDbContext, TEntity, TKey>> logger, 
        AppDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }
}
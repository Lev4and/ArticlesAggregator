using Database.EntityFramework.Repositories;
using Primitives;

namespace ProxyServerSearcher.Persistence;

public abstract class AppDbContextRepository<TEntity, TKey> : EntityFrameworkRepository<AppDbContext, TEntity, TKey>
    where TEntity : EntityBase<TKey>
    where TKey : notnull
{
    protected AppDbContextRepository(AppDbContext dbContext) : base(dbContext)
    {
        
    }
}
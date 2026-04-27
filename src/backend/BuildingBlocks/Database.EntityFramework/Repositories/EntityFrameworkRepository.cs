using Database.Abstracts;

using Extensions;

using Microsoft.EntityFrameworkCore;

using Primitives;

namespace Database.EntityFramework.Repositories;

public abstract class EntityFrameworkRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
    where TDbContext : BaseDbContext
    where TEntity : EntityBase<TKey> 
    where TKey : notnull
{
    protected readonly TDbContext DbContext;
    
    public EntityFrameworkRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public void Add(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        Add(entity);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().AddRange(entities);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        AddRange(entities);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> ContainsAsync(TKey id, CancellationToken ct = default)
    {
        return await DbContext.Set<TEntity>().AnyAsync(entity => entity.Id.Equals(id), ct);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(TKey id, CancellationToken ct = default)
    {
        return await DbContext.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id.Equals(id), ct);
    }

    public void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    public void Update(TEntity entity, Action<TEntity> updateAction)
    {
        updateAction(entity);
        
        Update(entity);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        Update(entity);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TEntity entity, Action<TEntity> updateAction, CancellationToken ct = default)
    {
        Update(entity, updateAction);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().UpdateRange(entities);
    }

    public void UpdateRange(IEnumerable<TEntity> entities, Action<TEntity> updateAction)
    {
        entities.ForEach(updateAction);
        
        UpdateRange(entities);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        UpdateRange(entities);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, Action<TEntity> updateAction, 
        CancellationToken ct = default)
    {
        UpdateRange(entities, updateAction);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public void Remove(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken ct = default)
    {
        Remove(entity);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().RemoveRange(entities);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        RemoveRange(entities);
        
        await DbContext.SaveChangesAsync(ct);
    }
}
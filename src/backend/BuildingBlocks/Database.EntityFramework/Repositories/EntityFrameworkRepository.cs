using Database.Abstracts;

using Extensions;

using Microsoft.EntityFrameworkCore;

using Primitives;

namespace Database.EntityFramework.Repositories;

public abstract class EntityFrameworkRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
    where TDbContext : BaseDbContext where TEntity : EntityBase<TKey> where TKey : struct
{
    private readonly TDbContext _dbContext;
    
    public EntityFrameworkRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Add(entity);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _dbContext.Set<TEntity>().AddRange(entities);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        AddRange(entities);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsync(TKey id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id.Equals(id), 
            cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
    }

    public void Update(TEntity entity, Action<TEntity> updateAction)
    {
        updateAction(entity);
        
        Update(entity);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Update(entity);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, Action<TEntity> updateAction, 
        CancellationToken cancellationToken = default)
    {
        Update(entity, updateAction);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbContext.Set<TEntity>().UpdateRange(entities);
    }

    public void UpdateRange(IEnumerable<TEntity> entities, Action<TEntity> updateAction)
    {
        entities.ForEach(updateAction);
        
        UpdateRange(entities);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        UpdateRange(entities);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, Action<TEntity> updateAction,
        CancellationToken cancellationToken = default)
    {
        UpdateRange(entities, updateAction);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Remove(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Remove(entity);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbContext.Set<TEntity>().RemoveRange(entities);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        RemoveRange(entities);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
using Database.Abstracts;

using Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using Primitives;

namespace Database.EntityFramework.Repositories;

public abstract class EntityFrameworkRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey> 
    where TDbContext : BaseDbContext where TEntity : EntityBase<TKey> where TKey : notnull
{
    protected readonly ITracer<EntityFrameworkRepository<TDbContext, TEntity, TKey>> Tracer;
    protected readonly ILogger<EntityFrameworkRepository<TDbContext, TEntity, TKey>> Logger;
    
    protected readonly TDbContext DbContext;
    
    public EntityFrameworkRepository(
        ITracer<EntityFrameworkRepository<TDbContext, TEntity, TKey>> tracer,
        ILogger<EntityFrameworkRepository<TDbContext, TEntity, TKey>> logger,
        TDbContext dbContext)
    {
        Tracer = tracer;
        Logger = logger;
        
        DbContext = dbContext;
    }

    public virtual void Add(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Add entity to change tracker");
        
        Logger.LogInformation("Add entity to change tracker");
        
        Add(entity);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        using var operation = Tracer.StartOperation("Add entities to change tracker");
        
        Logger.LogInformation("Add entities to change tracker");
        
        DbContext.Set<TEntity>().AddRange(entities);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Add entities to db");
        
        Logger.LogInformation("Add entities to db");
        
        AddRange(entities);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public virtual async Task<bool> ContainsAsync(TKey id, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Contains by id from db");
        
        Logger.LogInformation("Contains by id from db");
        
        return await DbContext.Set<TEntity>().AnyAsync(entity => entity.Id.Equals(id), ct);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(TKey id, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Find by id from db");
        
        Logger.LogInformation("Find by id from db");
        
        return await DbContext.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id.Equals(id), ct);
    }

    public virtual void Update(TEntity entity)
    {
        using var operation = Tracer.StartOperation("Update entity in change tracker");
        
        Logger.LogInformation("Update entity in change tracker");
        
        DbContext.Set<TEntity>().Update(entity);
    }

    public virtual void Update(TEntity entity, Action<TEntity> updateAction)
    {
        updateAction(entity);
        
        Update(entity);
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Update entity in db");
        
        Logger.LogInformation("Update entity in db");
        
        Update(entity);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateAsync(TEntity entity, Action<TEntity> updateAction, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Update entity in db");
        
        Logger.LogInformation("Update entity in db");
        
        Update(entity, updateAction);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        using var operation = Tracer.StartOperation("Update entities in change tracker");
        
        Logger.LogInformation("Update entities in change tracker");
        
        DbContext.Set<TEntity>().UpdateRange(entities);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities, Action<TEntity> updateAction)
    {
        entities.ForEach(updateAction);
        
        UpdateRange(entities);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Update entities in db");
        
        Logger.LogInformation("Update entities in db");
        
        UpdateRange(entities);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, Action<TEntity> updateAction, 
        CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Update entities in db");
        
        Logger.LogInformation("Update entities in db");
        
        UpdateRange(entities, updateAction);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public virtual void Remove(TEntity entity)
    {
        using var operation = Tracer.StartOperation("Remove entity from change tracker");
        
        Logger.LogInformation("Remove entity from change tracker");
        
        DbContext.Set<TEntity>().Remove(entity);
    }

    public virtual async Task RemoveAsync(TEntity entity, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Remove entity from db");
        
        Logger.LogInformation("Remove entity from db");
        
        Remove(entity);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        using var operation = Tracer.StartOperation("Remove entities from change tracker");
        
        Logger.LogInformation("Remove entities from change tracker");
        
        DbContext.Set<TEntity>().RemoveRange(entities);
    }

    public virtual async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Remove entities from db");
        
        Logger.LogInformation("Remove entities from db");
        
        RemoveRange(entities);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        DbContext.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}
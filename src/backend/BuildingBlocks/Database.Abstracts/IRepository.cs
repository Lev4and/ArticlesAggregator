using Primitives;

namespace Database.Abstracts;

public interface IRepository<TEntity, TKey>
    where TEntity : EntityBase<TKey> 
    where TKey : notnull
{
    void Add(TEntity entity);
    
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    
    void AddRange(IEnumerable<TEntity> entities);
    
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    
    Task<bool> ContainsAsync(TKey id, CancellationToken ct = default);
    
    Task<TEntity?> FirstOrDefaultAsync(TKey id, CancellationToken ct = default);
    
    void Update(TEntity entity);
    
    void Update(TEntity entity, Action<TEntity> updateAction);
    
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    
    Task UpdateAsync(TEntity entity, Action<TEntity> updateAction, CancellationToken ct = default);
    
    void UpdateRange(IEnumerable<TEntity> entities);
    
    void UpdateRange(IEnumerable<TEntity> entities, Action<TEntity> updateAction);
    
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, Action<TEntity> updateAction, CancellationToken ct = default);
    
    void Remove(TEntity entity);

    Task RemoveAsync(TEntity entity, CancellationToken ct = default);
    
    void RemoveRange(IEnumerable<TEntity> entities);
    
    Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
}
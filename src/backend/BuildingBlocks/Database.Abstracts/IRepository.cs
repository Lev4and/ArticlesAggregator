using Primitives;

namespace Database.Abstracts;

public interface IRepository<TEntity, TKey> 
    where TEntity : EntityBase<TKey> where TKey : struct
{
    void Add(TEntity entity);
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    void AddRange(IEnumerable<TEntity> entities);
    
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);
    
    void Update(TEntity entity);
    
    void Update(TEntity entity, Action<TEntity> updateAction);
    
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(TEntity entity, Action<TEntity> updateAction, 
        CancellationToken cancellationToken = default);
    
    void UpdateRange(IEnumerable<TEntity> entities);
    
    void UpdateRange(IEnumerable<TEntity> entities, Action<TEntity> updateAction);
    
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, Action<TEntity> updateAction,
        CancellationToken cancellationToken = default);
    
    void Remove(TEntity entity);

    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    void RemoveRange(IEnumerable<TEntity> entities);
    
    Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}
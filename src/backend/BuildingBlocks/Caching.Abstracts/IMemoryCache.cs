namespace Caching.Abstracts;

public interface IMemoryCache
{
    Task SetAsync<TData>(string key, TData data, CancellationToken cancellationToken = default);
    
    Task<TData?> GetAsync<TData>(string key, CancellationToken cancellationToken = default);
    
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
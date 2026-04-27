namespace Caching.Abstracts;

public interface IMemoryCache
{
    Task SetAsync<TData>(string key, TData data, CancellationToken ct = default);

    Task SetAsync<TData>(string key, TData data, CacheEntryOptions options, CancellationToken ct = default);
    
    Task<TData?> GetAsync<TData>(string key, CancellationToken ct = default);
    
    Task<TData> GetAsync<TData>(string key, Func<Task<TData>> factory, CacheEntryOptions? options = null, 
        CancellationToken ct = default);
    
    Task RemoveAsync(string key, CancellationToken ct = default);
}
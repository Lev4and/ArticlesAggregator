using Caching.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Caching.Redis;

public class RedisMemoryCache : IMemoryCache
{
    private readonly IDistributedCache _cache;
    
    public RedisMemoryCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync<TData>(string key, TData data, CancellationToken cancellationToken = default)
    {
        var dataJson = JsonConvert.SerializeObject(data);

        await _cache.SetStringAsync(key, dataJson, cancellationToken);
    }

    public async Task<TData?> GetAsync<TData>(string key, CancellationToken cancellationToken = default)
    {
        var dataJson = await _cache.GetStringAsync(key, cancellationToken);
        
        return !string.IsNullOrEmpty(dataJson) 
            ? JsonConvert.DeserializeObject<TData>(dataJson) 
            : default;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
}
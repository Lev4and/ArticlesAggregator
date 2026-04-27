using Caching.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Caching.Redis;

public class RedisMemoryCache : IMemoryCache
{
    private readonly IDistributedCache      _distributedCache;
    private readonly JsonSerializerSettings _serializerSettings;
    
    public RedisMemoryCache(
        IDistributedCache distributedCache)
    {
        _distributedCache   = distributedCache;
        _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public async Task SetAsync<TData>(string key, TData data, CancellationToken ct = default)
    {
        var dataJson = JsonConvert.SerializeObject(data, _serializerSettings);

        await _distributedCache.SetStringAsync(key, dataJson, ct);
    }

    public async Task SetAsync<TData>(string key, TData data, CacheEntryOptions options, CancellationToken ct = default)
    {
        var dataJson = JsonConvert.SerializeObject(data, _serializerSettings);

        await _distributedCache.SetStringAsync(
            key, 
            dataJson,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                SlidingExpiration  = options.SlidingExpiration
            },
            ct);
    }

    public async Task<TData?> GetAsync<TData>(string key, CancellationToken ct = default)
    {
        var dataJson = await _distributedCache.GetStringAsync(key, ct);
        
        return !string.IsNullOrEmpty(dataJson) 
            ? JsonConvert.DeserializeObject<TData>(dataJson, _serializerSettings) 
            : default;
    }

    public async Task<TData> GetAsync<TData>(string key, Func<Task<TData>> factory, CacheEntryOptions? options = null, 
        CancellationToken ct = default)
    {
        var data = await GetAsync<TData>(key, ct);
        if (data is not null)
        {
            return data;
        }

        data = await factory();
            
        if (options is null) await SetAsync(key, data, ct);
        else await SetAsync(key, data, options, ct);

        return data;
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _distributedCache.RemoveAsync(key, ct);
    }
}
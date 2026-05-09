using Caching.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Observability.Abstracts;

namespace Caching.Redis;

public class RedisMemoryCache : IMemoryCache
{
    private readonly ITracer<RedisMemoryCache> _tracer;
    private readonly ILogger<RedisMemoryCache> _logger;
    private readonly IDistributedCache         _distributedCache;
    private readonly JsonSerializerSettings    _serializerSettings;
    
    public RedisMemoryCache(
        ITracer<RedisMemoryCache> tracer,
        ILogger<RedisMemoryCache> logger, 
        IDistributedCache         distributedCache)
    {
        _tracer             = tracer;
        _logger             = logger;
        _distributedCache   = distributedCache;
        _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public async Task SetAsync<TData>(string key, TData data, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Set data in Redis cache");
        
        _logger.LogInformation("Set data in Redis cache Key: {CacheKey}", key);
        
        var dataJson = JsonConvert.SerializeObject(data, _serializerSettings);

        await _distributedCache.SetStringAsync(key, dataJson, ct);
    }

    public async Task SetAsync<TData>(string key, TData data, CacheEntryOptions options, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Set data in Redis cache");
        
        _logger.LogInformation("Set data in Redis cache Key: {CacheKey}", key);
        
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
        using var operation = _tracer.StartOperation("Get data from Redis cache");
        
        _logger.LogInformation("Get data from Redis cache Key: {CacheKey}", key);
        
        var dataJson = await _distributedCache.GetStringAsync(key, ct);
        
        return !string.IsNullOrEmpty(dataJson) 
            ? JsonConvert.DeserializeObject<TData>(dataJson, _serializerSettings) 
            : default;
    }

    public async Task<TData> GetAsync<TData>(string key, Func<Task<TData>> factory, CacheEntryOptions? options = null, 
        CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Get/Set data from Redis cache");
        
        _logger.LogInformation("Get/Set data from Redis cache Key: {CacheKey}", key);
        
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
        using var operation = _tracer.StartOperation("Remove data from Redis cache");
        
        _logger.LogInformation("Remove data from Redis cache Key: {CacheKey}", key);
        
        await _distributedCache.RemoveAsync(key, ct);
    }
}
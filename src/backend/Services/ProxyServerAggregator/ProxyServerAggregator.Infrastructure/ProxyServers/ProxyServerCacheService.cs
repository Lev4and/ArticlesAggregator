using Caching.Abstracts;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Application.Abstracts.ProxyServers;
using ProxyServerAggregator.Domain.Dtos.ProxyServers;

namespace ProxyServerAggregator.Infrastructure.ProxyServers;

public class ProxyServerCacheService : IProxyServerCacheService
{
    private const string ProxyServerKeyFormat = "proxy-server:{0}";
    
    private readonly ITracer<ProxyServerCacheService> _tracer;
    private readonly ILogger<ProxyServerCacheService> _logger;
    private readonly IMemoryCache _memoryCache;
    
    public ProxyServerCacheService(
        ITracer<ProxyServerCacheService> tracer, 
        ILogger<ProxyServerCacheService> logger, 
        IMemoryCache memoryCache)
    {
        _tracer = tracer;
        _logger = logger;
        _memoryCache = memoryCache;
    }
    
    public async Task SetAsync(ProxyServerDto proxyServer, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Set proxy server in cache");
        
        _logger.LogInformation("Set proxy server in cache Id: {ProxyServerId}", proxyServer.Id);
        
        await _memoryCache.SetAsync(
            string.Format(ProxyServerKeyFormat, proxyServer.Id), 
            proxyServer, 
            new CacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1),
            },
            ct);
    }

    public async Task<ProxyServerDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Get proxy server from cache");
        
        _logger.LogInformation("Get proxy server from cache Id: {ProxyServerId}", id);
        
        return await _memoryCache.GetAsync<ProxyServerDto>(string.Format(ProxyServerKeyFormat, id), ct);
    }
}
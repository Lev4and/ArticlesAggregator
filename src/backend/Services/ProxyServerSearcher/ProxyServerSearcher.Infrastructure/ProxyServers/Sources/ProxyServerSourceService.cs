using System.Reflection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using Result;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources;

public class ProxyServerSourceService : IProxyServerSourceService
{
    private readonly ITracer<ProxyServerSourceService> _tracer;
    private readonly ILogger<ProxyServerSourceService> _logger;
    
    public ProxyServerSourceService(
        ITracer<ProxyServerSourceService> tracer, 
        ILogger<ProxyServerSourceService> logger)
    {
        _tracer = tracer;
        _logger = logger;
    }
    
    public async Task<AppResult<string[]>> GetListAsync(CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Get proxy server source list");
        
        _logger.LogInformation("Get proxy server source list");
        
        var sourceList = typeof(ProxyServerSourceConstants)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field is { IsLiteral: true, IsInitOnly: false } && field.FieldType == typeof(string))
            .Select(fieldInfo => (string)fieldInfo.GetRawConstantValue()!)
            .ToArray();

        return await Task.FromResult(AppResult<string[]>.Success(sourceList));
    }
}
using Microsoft.Extensions.Logging;
using Missions.Abstracts;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Domain.Entities;
using StoredTasks.Database.Abstracts;

namespace ProxyServerSearcher.Application.UseCases.ProxyServers.Missions;

public class ProxyServerSearchPlanMission : IMission
{
    private readonly ITracer<ProxyServerSearchPlanMission> _tracer;
    private readonly ILogger<ProxyServerSearchPlanMission> _logger;
    private readonly IProxyServerSourceService _proxyServerSourceService;
    private readonly IStoredTaskRepository<ProxyServerSearchStoredTask> _repository;
    
    public ProxyServerSearchPlanMission(
        ITracer<ProxyServerSearchPlanMission> tracer, 
        ILogger<ProxyServerSearchPlanMission> logger,
        IProxyServerSourceService proxyServerSourceService,
        IStoredTaskRepository<ProxyServerSearchStoredTask> repository)
    {
        _tracer = tracer;
        _logger = logger;
        _proxyServerSourceService = proxyServerSourceService;
        _repository = repository;
    }
    
    public async Task RunAsync(CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server search plan mission");
        
        _logger.LogInformation("Proxy server search plan mission started");
        
        var listResult = await _proxyServerSourceService.GetListAsync(ct);
        if (listResult.IsFailure)
        {
            _logger.LogError("Proxy server search plan mission failed");
            
            return;
        }

        var storedTasks =
            listResult.Result!.Select(sourceName => 
                new ProxyServerSearchStoredTask
                {
                    SourceName = sourceName
                });
        
        await _repository.AddRangeAsync(storedTasks, ct);
        
        _logger.LogInformation("Proxy server search plan mission finished");
    }

    public void Dispose()
    {
        _repository.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _repository.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}
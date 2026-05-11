using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Domain.Entities;
using Result;
using StoredTasks.Abstracts;

namespace ProxyServerSearcher.Application.UseCases.ProxyServers.Tasks;

public class ProxyServerSearchStoredTaskHandler : IStoredTaskHandler<ProxyServerSearchStoredTask>
{
    private readonly ITracer<ProxyServerSearchStoredTaskHandler> _tracer;
    private readonly ILogger<ProxyServerSearchStoredTaskHandler> _logger;
    private readonly IProxyServerService _proxyServerService;
    private readonly IServiceProvider _serviceProvider;

    public ProxyServerSearchStoredTaskHandler(
        ITracer<ProxyServerSearchStoredTaskHandler> tracer, 
        ILogger<ProxyServerSearchStoredTaskHandler> logger,
        IProxyServerService proxyServerService,
        IServiceProvider serviceProvider)
    {
        _tracer = tracer;
        _logger = logger;
        _proxyServerService = proxyServerService;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<AppResult> HandleAsync(ProxyServerSearchStoredTask storedTask, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server search task handle");
        
        _logger.LogInformation("Proxy server search task handle Id: {StoredTaskId}", storedTask.Id);

        using var proxyServerSource = _serviceProvider.GetKeyedService<IProxyServerSource>(storedTask.SourceName);
        if (proxyServerSource is null)
        {
            _logger.LogError("Proxy server source not found Name: {ProxyServerSourceName}", storedTask.SourceName);

            return AppResult.Success();
        }

        await foreach (var models in proxyServerSource.ProvideAsync(ct))
        {
            var createResult = await _proxyServerService.CreateBatchAsync(models.ToArray(), ct);
            if (createResult.IsFailure)
            {
                var error = createResult.Errors.First();
                
                _logger.LogError(
                    "Proxy servers create failed ErrorType: {ErrorType} ErrorMessage: {ErrorMessage}",
                        error.Type, error.Message);
                
                break;
            }
        }
        
        return AppResult.Success();
    }
}
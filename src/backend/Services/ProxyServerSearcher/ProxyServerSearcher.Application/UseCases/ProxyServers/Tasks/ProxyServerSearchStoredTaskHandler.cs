using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Domain.Entities;
using ProxyServerSearcher.Domain.Repositories;
using ProxyServerSearcher.Domain.ValueObjects;
using Result;
using StoredTasks.Abstracts;

namespace ProxyServerSearcher.Application.UseCases.ProxyServers.Tasks;

public class ProxyServerSearchStoredTaskHandler : IStoredTaskHandler<ProxyServerSearchStoredTask>
{
    private readonly ITracer<ProxyServerSearchStoredTaskHandler> _tracer;
    private readonly ILogger<ProxyServerSearchStoredTaskHandler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IProxyServerRepository _repository;
    
    public ProxyServerSearchStoredTaskHandler(
        ITracer<ProxyServerSearchStoredTaskHandler> tracer, 
        ILogger<ProxyServerSearchStoredTaskHandler> logger, 
        IServiceProvider serviceProvider, 
        IProxyServerRepository repository)
    {
        _tracer = tracer;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _repository = repository;
    }
    
    public async Task<AppResult> HandleAsync(ProxyServerSearchStoredTask storedTask, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server search task handle");
        
        _logger.LogInformation("Proxy server search task handle Id: {StoredTaskId}", storedTask.Id);

        var proxyServerSource = _serviceProvider.GetKeyedService<IProxyServerSource>(storedTask.SourceName);
        if (proxyServerSource is null)
        {
            _logger.LogError("Proxy server source not found Name: {ProxyServerSourceName}", storedTask.SourceName);

            return AppResult.Success();
        }

        await foreach (var proxyServerModels in proxyServerSource.ProvideAsync(ct))
        {
            var proxyServerNames = proxyServerModels
                .Select(proxyServer => proxyServer.NormalizedName)
                    .ToArray();

            var oldProxyServers = await _repository.GetExistsAsync(proxyServerNames, ct);
            var newProxyServers = proxyServerModels
                .Where(proxyServer => !oldProxyServers.Contains(proxyServer.NormalizedName))
                .Select(proxyServer =>
                    new ProxyServer
                    {
                        NormalizedName    = proxyServer.NormalizedName,
                        Protocol          = proxyServer.Protocol,
                        HostnameOrAddress = proxyServer.HostnameOrAddress,
                        Port              = proxyServer.Port,
                        Credentials       = proxyServer.Credentials is not null
                            ? new ProxyServerCredentials
                            {
                                Username = proxyServer.Credentials.Username, 
                                Password = proxyServer.Credentials.Password
                            }
                            : null
                    });
            
            await _repository.AddRangeAsync(newProxyServers, ct);
        }
        
        return AppResult.Success();
    }
}
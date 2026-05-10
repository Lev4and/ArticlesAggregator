using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging.Pulsar;

public class PulsarConsumerWorkerService : BackgroundService
{
    private readonly ILogger<PulsarConsumerWorkerService> _logger;
    private readonly IServiceScopeFactory                 _serviceScopeFactory;
    private readonly string                               _consumerWorkerKey;
    
    public PulsarConsumerWorkerService(
        ILogger<PulsarConsumerWorkerService> logger, 
        IServiceScopeFactory                 serviceScopeFactory,
        string                               consumerWorkerKey)
    {
        _logger              = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _consumerWorkerKey   = consumerWorkerKey;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Pulsar consumer worker service started");
        
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        
        var consumerWorker = scope.ServiceProvider.GetRequiredKeyedService<PulsarConsumerWorker>(_consumerWorkerKey);
        
        await consumerWorker.RunAsync(stoppingToken);
        
        _logger.LogInformation("Pulsar consumer worker service finished");
    }
}
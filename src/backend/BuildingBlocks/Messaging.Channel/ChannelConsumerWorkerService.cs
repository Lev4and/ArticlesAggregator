using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging.Channel;

public class ChannelConsumerWorkerService : BackgroundService
{
    private readonly ILogger<ChannelConsumerWorkerService> _logger;
    private readonly IServiceScopeFactory                  _serviceScopeFactory;
    private readonly string                                _consumerWorkerKey;
    
    public ChannelConsumerWorkerService(
        ILogger<ChannelConsumerWorkerService> logger, 
        IServiceScopeFactory                  serviceScopeFactory,
        string                                consumerWorkerKey)
    {
        _logger              = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _consumerWorkerKey   = consumerWorkerKey;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Channel consumer worker service started");
        
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        
        var consumerWorker = scope.ServiceProvider.GetRequiredKeyedService<ChannelConsumerWorker>(_consumerWorkerKey);
        
        await consumerWorker.RunAsync(stoppingToken);
        
        _logger.LogInformation("Channel consumer worker service finished");
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerTester.Domain.Entities;
using StoredTasks.Database.Missions;

namespace ProxyServerTester.Application.UseCases.ProxyServers.Missions;

public class ProxyServerTestStoredTaskMission : StoredTaskMission<ProxyServerTestStoredTask>
{
    protected override int TaskLimit => 10;

    protected override TimeSpan AttemptDuration => TimeSpan.FromMinutes(5);
    
    protected override int WorkerCount => 50;
    
    public ProxyServerTestStoredTaskMission(
        ITracer<StoredTaskMission<ProxyServerTestStoredTask>> tracer, 
        ILogger<StoredTaskMission<ProxyServerTestStoredTask>> logger, 
        IServiceScopeFactory serviceScopeFactory) : 
        base(tracer, logger, serviceScopeFactory)
    {
        
    }
}
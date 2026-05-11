using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Domain.Entities;
using StoredTasks.Database.Abstracts;
using StoredTasks.Database.Missions;

namespace ProxyServerSearcher.Application.UseCases.ProxyServers.Missions;

public class ProxyServerSearchStoredTaskMission : StoredTaskMission<ProxyServerSearchStoredTask>
{
    protected override int TaskLimit => 1;

    protected override TimeSpan AttemptDuration => TimeSpan.FromMinutes(10);
    
    protected override int WorkerCount => 2;

    public ProxyServerSearchStoredTaskMission(
        ITracer<StoredTaskMission<ProxyServerSearchStoredTask>> tracer, 
        ILogger<StoredTaskMission<ProxyServerSearchStoredTask>> logger, 
        IStoredTaskRepository<ProxyServerSearchStoredTask> repository, 
        IServiceProvider serviceProvider) : 
        base(tracer, logger, repository, serviceProvider)
    {
        
    }
}
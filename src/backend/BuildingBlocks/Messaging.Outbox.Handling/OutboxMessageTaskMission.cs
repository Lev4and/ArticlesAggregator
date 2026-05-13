using Messaging.Outbox.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using StoredTasks.Database.Abstracts;
using StoredTasks.Database.Missions;

namespace Messaging.Outbox.Handling;

public class OutboxMessageTaskMission : StoredTaskMission<OutboxMessage>
{
    protected override int TaskLimit => 10;

    protected override TimeSpan AttemptDuration => TimeSpan.FromMinutes(1);

    protected override int WorkerCount => 2;

    public OutboxMessageTaskMission(
        ITracer<StoredTaskMission<OutboxMessage>> tracer, 
        ILogger<StoredTaskMission<OutboxMessage>> logger, 
        IServiceScopeFactory serviceScopeFactory) : 
        base(tracer, logger, serviceScopeFactory)
    {
        
    }
}
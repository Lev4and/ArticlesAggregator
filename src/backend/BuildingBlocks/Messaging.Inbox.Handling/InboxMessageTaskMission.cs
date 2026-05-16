using Messaging.Inbox.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using StoredTasks.Database.Missions;

namespace Messaging.Inbox.Handling;

public class InboxMessageTaskMission : StoredTaskMission<InboxMessage>
{
    protected override int TaskLimit => 10;

    protected override TimeSpan AttemptDuration => TimeSpan.FromMinutes(1);

    protected override int WorkerCount => 2;
    
    public InboxMessageTaskMission(
        ITracer<StoredTaskMission<InboxMessage>> tracer, 
        ILogger<StoredTaskMission<InboxMessage>> logger, 
        IServiceScopeFactory serviceScopeFactory) : 
        base(tracer, logger, serviceScopeFactory)
    {
        
    }
}
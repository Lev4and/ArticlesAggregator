using Database.EntityFramework;
using Database.EntityFramework.Repositories;
using Messaging.Abstracts;
using Messaging.Outbox.Abstracts;
using Messaging.Outbox.Abstracts.Extensions;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using StoredTasks.Database.EntityFramework.Repositories;

namespace Messaging.Outbox.EntityFramework;

public class OutboxMessageRepository<TDbContext> : StoredTaskRepository<TDbContext, OutboxMessage>, 
    IOutboxMessageRepository
    where TDbContext : BaseDbContext
{
    public OutboxMessageRepository(
        ITracer<EntityFrameworkRepository<TDbContext, OutboxMessage, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<TDbContext, OutboxMessage, Guid>> logger, 
        TDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }

    public virtual void AddMessage<TMessage>(TMessage message) 
        where TMessage : IMessage
    {
        using var operation = Tracer.StartOperation("Add outbox message to change tracker");
        
        Logger.LogInformation("Add outbox message to change tracker");
        
        base.Add(message.ToOutboxMessage());
    }

    public virtual async Task AddMessageAsync<TMessage>(TMessage message, CancellationToken ct = default) 
        where TMessage : IMessage
    {
        using var operation = Tracer.StartOperation("Add outbox message to db");
        
        Logger.LogInformation("Add outbox message to db");
        
        await base.AddAsync(message.ToOutboxMessage(), ct);
    }

    public virtual void AddRangeMassage<TMessage>(IEnumerable<TMessage> messages) 
        where TMessage : IMessage
    {
        using var operation = Tracer.StartOperation("Add outbox messages to change tracker");
        
        Logger.LogInformation("Add outbox messages to change tracker");
        
        base.AddRange(messages.Select(message => message.ToOutboxMessage()));
    }

    public virtual async Task AddRangeMassageAsync<TMessage>(IEnumerable<TMessage> messages, 
        CancellationToken ct = default) 
        where TMessage : IMessage
    {
        using var operation = Tracer.StartOperation("Add outbox messages to db");
        
        Logger.LogInformation("Add outbox messages to db");
        
        await base.AddRangeAsync(messages.Select(message => message.ToOutboxMessage()), ct);
    }
}
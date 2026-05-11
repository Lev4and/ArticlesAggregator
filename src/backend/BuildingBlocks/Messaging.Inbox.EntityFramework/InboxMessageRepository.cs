using Database.EntityFramework;
using Database.EntityFramework.Repositories;
using Messaging.Abstracts;
using Messaging.Inbox.Abstracts;
using Messaging.Inbox.EntityFramework.Extensions;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using StoredTasks.Database.EntityFramework.Repositories;

namespace Messaging.Inbox.EntityFramework;

public class InboxMessageRepository<TDbContext> : StoredTaskRepository<TDbContext, InboxMessage>, 
    IInboxMessageRepository
    where TDbContext : BaseDbContext
{
    public InboxMessageRepository(
        ITracer<EntityFrameworkRepository<TDbContext, InboxMessage, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<TDbContext, InboxMessage, Guid>> logger, 
        TDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }

    public virtual void Add<TMessage>(TMessage message) 
        where TMessage : IMessage
    {
        using var operation = Tracer.StartOperation("Add inbox message to change tracker");
        
        Logger.LogInformation("Add inbox message to change tracker");
        
        base.Add(message.ToInboxMessage());
    }

    public virtual async Task AddAsync<TMessage>(TMessage message, CancellationToken ct = default) 
        where TMessage : IMessage
    {
        using var operation = Tracer.StartOperation("Add inbox message to db");
        
        Logger.LogInformation("Add inbox message to db");
        
        await base.AddAsync(message.ToInboxMessage(), ct);
    }

    public virtual void AddRange<TMessage>(IEnumerable<TMessage> messages) 
        where TMessage : IMessage
    {
        using var operation = Tracer.StartOperation("Add inbox messages to change tracker");
        
        Logger.LogInformation("Add inbox messages to change tracker");
        
        base.AddRange(messages.Select(message => message.ToInboxMessage()));
    }

    public virtual async Task AddRangeAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken ct = default) 
        where TMessage : IMessage
    {
        using var operation = Tracer.StartOperation("Add inbox messages to db");
        
        Logger.LogInformation("Add inbox messages to db");
        
        await base.AddRangeAsync(messages.Select(message => message.ToInboxMessage()), ct);
    }
}
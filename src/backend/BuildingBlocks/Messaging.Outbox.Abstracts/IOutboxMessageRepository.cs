using Messaging.Abstracts;
using StoredTasks.Database.Abstracts;

namespace Messaging.Outbox.Abstracts;

public interface IOutboxMessageRepository : IStoredTaskRepository<OutboxMessage>
{
    void Add<TMessage>(TMessage message)
        where TMessage : IMessage;
    
    Task AddAsync<TMessage>(TMessage message, CancellationToken ct = default)
        where TMessage : IMessage;
    
    // void AddRange<TMessage>(IEnumerable<TMessage> messages)
    //     where TMessage : IMessage;
    //
    // Task AddRangeAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken ct = default)
    //     where TMessage : IMessage;
}
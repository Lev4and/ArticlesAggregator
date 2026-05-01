using Result;

namespace DomainEvents.Abstracts;

public interface IDomainEventHandler<TDomainEvent> : IDomainEventHandler
    where TDomainEvent : IDomainEvent
{
    Task<AppResult> HandleAsync(TDomainEvent domainEvent, CancellationToken ct = default);

    Task<AppResult> IDomainEventHandler.HandleAsync(IDomainEvent domainEvent, CancellationToken ct)
    {
        return HandleAsync((TDomainEvent)domainEvent, ct);
    }
}

public interface IDomainEventHandler
{
    Task<AppResult> HandleAsync(IDomainEvent domainEvent, CancellationToken ct = default);
}
using Result;

namespace DomainEvents.Abstracts;

public interface IDomainEventHandler<TDomainEvent> : IDomainEventHandler
    where TDomainEvent : IDomainEvent
{
    Task<AppResult> HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);

    Task<AppResult> IDomainEventHandler.HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        return HandleAsync((TDomainEvent)domainEvent, cancellationToken);
    }
}

public interface IDomainEventHandler
{
    Task<AppResult> HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
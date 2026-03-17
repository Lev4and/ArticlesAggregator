using DomainEvents.Abstracts;
using Primitives;

namespace DomainEvents.Database.Abstracts;

public abstract class AggregateRoot<TKey> : EntityBase<TKey>, IHasDomainEvents
    where TKey : struct
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IEnumerable<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
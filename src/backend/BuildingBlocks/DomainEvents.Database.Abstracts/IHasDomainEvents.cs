using DomainEvents.Abstracts;

namespace DomainEvents.Database.Abstracts;

public interface IHasDomainEvents
{
    void AddDomainEvent(IDomainEvent domainEvent);
    
    IEnumerable<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}
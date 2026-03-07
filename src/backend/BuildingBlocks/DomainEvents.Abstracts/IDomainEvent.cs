namespace DomainEvents.Abstracts;

public interface IDomainEvent
{
    Guid Id { get; }
    
    DateTime CreatedAt { get; }
}
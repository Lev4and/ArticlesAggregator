using System.Text.Json;
using Contracts;
using DomainEvents.Abstracts;
using Primitives;

namespace DomainEvents.Database.Abstracts;

public class DomainEvent : EntityBase<Guid>, IHasEntityState, IHasTimestamps, IHasSoftDelete
{
    public string Type { get; set; } = null!;

    public JsonDocument Content { get; set; } = null!;
    
    public EntityState EntityState { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }

    public static DomainEvent Create(IDomainEvent domainEvent)
    {
        var domainEventType = domainEvent.GetType();

        var entity = new DomainEvent
        {
            Id = domainEvent.Id, 
            Type = domainEventType.Name,
            Content = JsonSerializer.SerializeToDocument(domainEvent, domainEventType),
        };
        
        return entity;
    }
}
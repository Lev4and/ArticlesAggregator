using Messaging.Abstracts;

namespace Messaging.Messages;

public abstract record BaseMessage : IMessage
{
    public Guid Id { get; init; }
    
    public DateTime CreatedAt { get; init; }

    public BaseMessage()
    {
        Id        = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public BaseMessage(Guid id, DateTime createdAt)
    {
        Id        = id;
        CreatedAt = createdAt;
    }
}
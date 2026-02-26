namespace Messaging.Abstracts;

public interface IMessage
{
    Guid Id { get; }
    
    DateTime CreatedAt { get; }
}
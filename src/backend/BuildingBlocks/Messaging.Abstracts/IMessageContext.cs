namespace Messaging.Abstracts;

public interface IMessageContext
{
    string MessageId { get; }
    
    IMessage Data { get; }
    
    DateTime PublishedAt { get; }
}
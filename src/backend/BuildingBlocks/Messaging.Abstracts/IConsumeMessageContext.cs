namespace Messaging.Abstracts;

public interface IConsumeMessageContext : IMessageContext 
{
    DateTime ConsumedAt { get; }
}
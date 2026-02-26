namespace Messaging.Abstracts;

public interface IMessageHandler<TMessage> : IMessageHandler
    where TMessage : IMessage
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    
    Task IMessageHandler.HandleAsync(IMessage message, CancellationToken cancellationToken)
    {
        return HandleAsync((TMessage)message, cancellationToken);
    }

}

public interface IMessageHandler 
{
    Task HandleAsync(IMessage message, CancellationToken cancellationToken = default);
}
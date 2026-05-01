using Result;

namespace Messaging.Abstracts;

public interface IMessageHandler<TMessage> : IMessageHandler
    where TMessage : IMessage
{
    Task<AppResult> HandleAsync(TMessage message, CancellationToken ct = default);
    
    Task<AppResult> IMessageHandler.HandleAsync(IMessage message, CancellationToken ct)
    {
        return HandleAsync((TMessage)message, ct);
    }

}

public interface IMessageHandler 
{
    Task<AppResult> HandleAsync(IMessage message, CancellationToken ct = default);
}
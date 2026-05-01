namespace Messaging.Abstracts;

public interface IMessageProducer : IAsyncDisposable
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken ct = default)
        where TMessage : IMessage;
}
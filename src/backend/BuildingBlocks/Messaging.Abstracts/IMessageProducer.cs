namespace Messaging.Abstracts;

public interface IMessageProducer
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage;
}
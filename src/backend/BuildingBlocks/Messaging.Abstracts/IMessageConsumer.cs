namespace Messaging.Abstracts;

public interface IMessageConsumer
{
    IAsyncEnumerable<IMessage> ReceiveAsync(CancellationToken cancellationToken = default);
}
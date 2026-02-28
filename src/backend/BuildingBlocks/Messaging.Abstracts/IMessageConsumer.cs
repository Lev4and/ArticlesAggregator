namespace Messaging.Abstracts;

public interface IMessageConsumer
{
    IAsyncEnumerable<IConsumeMessageContext> ReceiveAsync(CancellationToken cancellationToken = default);
    
    Task AcknowledgeAsync(IConsumeMessageContext context, CancellationToken cancellationToken = default);
    
    Task NegativeAcknowledgeAsync(IConsumeMessageContext context, CancellationToken cancellationToken = default);
}
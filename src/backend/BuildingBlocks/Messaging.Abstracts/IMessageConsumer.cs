namespace Messaging.Abstracts;

public interface IMessageConsumer : IAsyncDisposable
{
    IAsyncEnumerable<IConsumeMessageContext> ReceiveAsync(CancellationToken ct = default);
    
    Task AcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default);
    
    Task NegativeAcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default);
}
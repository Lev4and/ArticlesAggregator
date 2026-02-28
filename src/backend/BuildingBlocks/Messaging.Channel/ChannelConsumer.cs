using System.Runtime.CompilerServices;
using Messaging.Abstracts;

namespace Messaging.Channel;

public class ChannelConsumer : IMessageConsumer
{
    private readonly ChannelQueue _queue;
    
    public ChannelConsumer(ChannelQueue queue)
    {
        _queue = queue;
    }
    
    public async IAsyncEnumerable<IConsumeMessageContext> ReceiveAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            while (await _queue.Reader.WaitToReadAsync(cancellationToken))
            {
                var messageContext = await _queue.Reader.ReadAsync(cancellationToken);
                
                yield return new ChannelConsumeMessageContext(messageContext, DateTime.UtcNow);
            }
        }
    }

    public Task AcknowledgeAsync(IConsumeMessageContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task NegativeAcknowledgeAsync(IConsumeMessageContext context, 
        CancellationToken cancellationToken = default)
    {
        await _queue.Writer.WriteAsync(context, cancellationToken);
    }
}
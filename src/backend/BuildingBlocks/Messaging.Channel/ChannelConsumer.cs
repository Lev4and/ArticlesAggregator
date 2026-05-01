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
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            while (await _queue.Reader.WaitToReadAsync(ct))
            {
                var messageContext = await _queue.Reader.ReadAsync(ct);
                
                yield return new ChannelConsumeMessageContext
                {
                    MessageId   = messageContext.MessageId,
                    Data        = messageContext.Data,
                    PublishedAt = messageContext.PublishedAt,
                    ConsumedAt  = DateTime.UtcNow
                };
            }
        }
    }

    public async Task AcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default)
    {
        await Task.CompletedTask;
    }

    public async Task NegativeAcknowledgeAsync(IConsumeMessageContext context, CancellationToken ct = default)
    {
        await _queue.Writer.WriteAsync(context, ct);
    }

    public async ValueTask DisposeAsync()
    {
        // TODO release managed resources here
    }
}
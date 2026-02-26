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
    
    public async IAsyncEnumerable<IMessage> ReceiveAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            while (await _queue.Reader.WaitToReadAsync(cancellationToken))
            {
                yield return await _queue.Reader.ReadAsync(cancellationToken);
            }
        }
    }
}
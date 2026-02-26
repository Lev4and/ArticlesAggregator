using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Messaging.Abstracts;
using IMessage = Messaging.Abstracts.IMessage;

namespace Messaging.Pulsar;

public class PulsarConsumer : IMessageConsumer
{
    private readonly IPulsarClient _client;
    
    public PulsarConsumer(IPulsarClient client)
    {
        _client = client;
    }
    
    public IAsyncEnumerable<IMessage> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
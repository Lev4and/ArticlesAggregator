using System.Threading.Channels;
using Messaging.Abstracts;

namespace Messaging.Channel;

public class ChannelQueue
{
    private readonly Channel<IMessageContext> _channel = System.Threading.Channels.Channel.CreateUnbounded<IMessageContext>();
    
    public ChannelWriter<IMessageContext> Writer => _channel.Writer;
    
    public ChannelReader<IMessageContext> Reader => _channel.Reader;

}
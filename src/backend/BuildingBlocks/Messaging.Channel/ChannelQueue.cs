using System.Threading.Channels;
using Messaging.Abstracts;

namespace Messaging.Channel;

public class ChannelQueue
{
    private readonly Channel<IMessage> _channel = System.Threading.Channels.Channel.CreateUnbounded<IMessage>();
    
    public ChannelWriter<IMessage> Writer => _channel.Writer;
    
    public ChannelReader<IMessage> Reader => _channel.Reader;

}
using System.Collections.Concurrent;

namespace Messaging.Channel;

public class ChannelQueueDictionary : ConcurrentDictionary<string, ChannelQueue>
{
    
}
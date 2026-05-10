using System.Collections.Concurrent;
using Messaging.Abstracts;

namespace Messaging.Channel;

public class ChannelQueueDictionary : ConcurrentDictionary<MessageTopic, ChannelQueue>
{
    
}
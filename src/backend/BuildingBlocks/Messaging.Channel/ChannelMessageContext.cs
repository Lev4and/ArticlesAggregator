using Messaging.Abstracts;

namespace Messaging.Channel;

public record ChannelMessageContext(string MessageId, IMessage Data, DateTime PublishedAt) : IMessageContext;
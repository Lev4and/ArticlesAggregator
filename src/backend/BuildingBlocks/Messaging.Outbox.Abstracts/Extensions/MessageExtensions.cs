using System.Text.Json;
using Messaging.Abstracts;
using Messaging.Messages;

namespace Messaging.Outbox.Abstracts.Extensions;

public static class MessageExtensions
{
    public static OutboxMessage ToOutboxMessage<TMessage>(this TMessage message)
        where TMessage : IMessage
    {
        return new OutboxMessage
        {
            Type    = MessageTypeResolver.Resolve(message) ?? throw new NotSupportedException(),
            Content = JsonSerializer.SerializeToDocument(message),
        };
    }
}
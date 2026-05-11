using System.Text.Json;
using Messaging.Abstracts;
using Messaging.Outbox.Abstracts;

namespace Messaging.Outbox.EntityFramework.Extensions;

public static class MessageExtensions
{
    public static OutboxMessage ToOutboxMessage<TMessage>(this TMessage message)
        where TMessage : IMessage
    {
        return new OutboxMessage
        {
            Type    = typeof(TMessage).FullName!,
            Content = JsonSerializer.SerializeToDocument(message),
        };
    }
}
using System.Text.Json;
using Messaging.Abstracts;
using Messaging.Inbox.Abstracts;

namespace Messaging.Inbox.EntityFramework.Extensions;

public static class MessageExtensions
{
    public static InboxMessage ToInboxMessage<TMessage>(this TMessage message)
        where TMessage : IMessage
    {
        return new InboxMessage
        {
            Type    = typeof(TMessage).Name,
            Content = JsonSerializer.SerializeToDocument(message),
        };
    }
}
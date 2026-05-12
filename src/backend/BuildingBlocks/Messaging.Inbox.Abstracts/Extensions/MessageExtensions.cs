using System.Text.Json;
using Messaging.Abstracts;

namespace Messaging.Inbox.Abstracts.Extensions;

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
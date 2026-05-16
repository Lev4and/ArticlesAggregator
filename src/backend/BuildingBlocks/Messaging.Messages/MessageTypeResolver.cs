using System.Reflection;
using Messaging.Abstracts;
using Messaging.Abstracts.Attributes;

namespace Messaging.Messages;

public class MessageTypeResolver
{
    public static Type? Resolve(string messageType)
    {
        return typeof(BaseMessage).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && type.IsAssignableTo(typeof(BaseMessage)))
            .FirstOrDefault(type => type.GetCustomAttribute<MessageTypeAttribute>()?.Type == messageType);
    }

    public static string? Resolve<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        return message.GetType().GetCustomAttribute<MessageTypeAttribute>()?.Type;
    }
}
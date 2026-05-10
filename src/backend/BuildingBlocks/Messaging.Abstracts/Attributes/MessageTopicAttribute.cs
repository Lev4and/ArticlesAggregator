namespace Messaging.Abstracts.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MessageTopicAttribute : Attribute
{
    public MessageTopic Topic { get; }

    public MessageTopicAttribute(MessageTopic topic)
    {
        Topic = topic;
    }
}
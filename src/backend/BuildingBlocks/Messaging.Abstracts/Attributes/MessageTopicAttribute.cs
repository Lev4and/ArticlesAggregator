namespace Messaging.Abstracts.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MessageTopicAttribute : Attribute
{
    public string Topic { get; }

    public MessageTopicAttribute(string topic)
    {
        Topic = topic;
    }
}
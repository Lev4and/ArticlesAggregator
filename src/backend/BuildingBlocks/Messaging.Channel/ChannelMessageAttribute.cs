namespace Messaging.Channel;

[AttributeUsage(AttributeTargets.Class)]
public class ChannelMessageAttribute : Attribute
{
    public string Topic { get; }

    public ChannelMessageAttribute(string topic)
    {
        Topic = topic;
    }
}
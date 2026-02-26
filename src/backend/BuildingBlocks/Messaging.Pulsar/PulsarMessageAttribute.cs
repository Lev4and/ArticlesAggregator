namespace Messaging.Pulsar;

[AttributeUsage(AttributeTargets.Class)]
public class PulsarMessageAttribute : Attribute
{
    public string Topic { get; }
    
    public PulsarMessageAttribute(string topic)
    {
        Topic = topic;
    }
}
namespace Messaging.Abstracts.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MessageTypeAttribute : Attribute
{
    public string Type { get; }
    
    public MessageTypeAttribute(string type)
    {
        Type = type;
    }
}
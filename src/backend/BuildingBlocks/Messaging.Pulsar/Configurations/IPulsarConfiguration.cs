namespace Messaging.Pulsar.Configurations;

public interface IPulsarConfiguration
{
    Uri Url { get; }
    
    bool UseTls { get; }
    
    string? AuthToken { get; }
}
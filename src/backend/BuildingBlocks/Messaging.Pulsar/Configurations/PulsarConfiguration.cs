namespace Messaging.Pulsar.Configurations;

public class PulsarConfiguration : IPulsarConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string PULSAR_URL = nameof(PULSAR_URL);
    
    // ReSharper restore InconsistentNaming

    public Uri Url => new(Environment.GetEnvironmentVariable(PULSAR_URL) ?? "pulsar://localhost:6650");
}
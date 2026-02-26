namespace Messaging.Pulsar.Configurations;

public class PulsarConfiguration : IPulsarConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string PULSAR_URL = nameof(PULSAR_URL);
    
    // ReSharper restore InconsistentNaming

    
    public Uri Url => new(Environment.GetEnvironmentVariable(PULSAR_URL) ?? 
        throw new ArgumentException($"{nameof(PULSAR_URL)} environment variable is not set."));
}
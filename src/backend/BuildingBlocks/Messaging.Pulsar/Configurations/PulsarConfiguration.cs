namespace Messaging.Pulsar.Configurations;

public class PulsarConfiguration : IPulsarConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string PULSAR_URL = nameof(PULSAR_URL);
    private const string PULSAR_USE_TLS = nameof(PULSAR_USE_TLS);
    private const string PULSAR_AUTH_TOKEN = nameof(PULSAR_AUTH_TOKEN);
    
    // ReSharper restore InconsistentNaming

    public Uri Url => new(Environment.GetEnvironmentVariable(PULSAR_URL) ?? "pulsar://localhost:6650");
    
    public bool UseTls => bool.Parse(Environment.GetEnvironmentVariable(PULSAR_USE_TLS) ?? "false");
    
    public string? AuthToken => Environment.GetEnvironmentVariable(PULSAR_AUTH_TOKEN);
}
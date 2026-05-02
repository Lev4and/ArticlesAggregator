namespace Observability.OpenTelemetry.Configurations;

public class OpenTelemetryConfiguration : IOpenTelemetryConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string OPENTELEMETRY_COLLECTOR_GRPC_URL = nameof(OPENTELEMETRY_COLLECTOR_GRPC_URL);
    
    // ReSharper restore InconsistentNaming


    public Uri OpenTelemetryCollectorGrpcUrl =>
        new(Environment.GetEnvironmentVariable(OPENTELEMETRY_COLLECTOR_GRPC_URL) ?? "http://localhost:4317");
}
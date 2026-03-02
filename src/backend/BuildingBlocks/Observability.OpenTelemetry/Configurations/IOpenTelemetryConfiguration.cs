namespace Observability.OpenTelemetry.Configurations;

public interface IOpenTelemetryConfiguration
{
    Uri OpenTelemetryCollectorGrpcUrl { get; }
}
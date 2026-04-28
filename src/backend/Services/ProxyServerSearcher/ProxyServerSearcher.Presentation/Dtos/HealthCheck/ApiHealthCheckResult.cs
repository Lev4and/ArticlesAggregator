using System.Text.Json.Serialization;

namespace ProxyServerSearcher.Presentation.Dtos.HealthCheck;

/// <summary>
/// Health check result
/// </summary>
public record ApiHealthCheckResult
{
    /// <summary>
    /// Health check status
    /// </summary>
    public string Status { get; init; } = null!;

    /// <summary>
    /// Unhealth services
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Service { get; init; }
}
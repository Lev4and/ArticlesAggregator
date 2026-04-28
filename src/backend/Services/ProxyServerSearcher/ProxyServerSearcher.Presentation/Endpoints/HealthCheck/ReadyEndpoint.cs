using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProxyServerSearcher.Presentation.Constants;
using ProxyServerSearcher.Presentation.Dtos.HealthCheck;

namespace ProxyServerSearcher.Presentation.Endpoints.HealthCheck;

public class ReadyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/ready",
                async (
                    [FromServices] HealthCheckService healthCheckService,
                    CancellationToken ct = default) =>
                {
                    var healthCheckResult =
                        await healthCheckService.CheckHealthAsync(x => x.Tags.Contains("ready"), ct);
                    if (healthCheckResult.Status is HealthStatus.Healthy)
                    {
                        return Results.Ok(new ApiHealthCheckResult { Status = "ok" });
                    }
                    
                    return Results.Json(
                        healthCheckResult.Entries
                            .Select(x => 
                                new ApiHealthCheckResult
                                {
                                    Status  = x.Value.Status is not HealthStatus.Healthy 
                                        ? x.Value.Description ?? "degraded"
                                        : "ok",
                                    Service = x.Key
                                })
                            .ToArray(),
                        statusCode: StatusCodes.Status503ServiceUnavailable);
                })
            .WithTags(EndpointTagConstants.HealthCheck)
            .WithName("Ready")
            .AllowAnonymous()
            .Produces<ApiHealthCheckResult>()
            .Produces<ApiHealthCheckResult[]>(statusCode: StatusCodes.Status503ServiceUnavailable)
            .WithSummary("Checking the service's readiness to serve requests")
            .WithDescription("Checking the service's readiness to serve requests");
    }
}
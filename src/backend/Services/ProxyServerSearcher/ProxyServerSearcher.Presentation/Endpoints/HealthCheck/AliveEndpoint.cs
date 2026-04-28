using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProxyServerSearcher.Presentation.Constants;
using ProxyServerSearcher.Presentation.Dtos.HealthCheck;

namespace ProxyServerSearcher.Presentation.Endpoints.HealthCheck;

public class AliveEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/alive",
                async (
                    [FromServices] HealthCheckService healthCheckService,
                    CancellationToken ct = default) =>
                {
                    var healthCheckResult =
                        await healthCheckService.CheckHealthAsync(x => x.Tags.Contains("alive"), ct);
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
            .WithName("Alive")
            .AllowAnonymous()
            .Produces<ApiHealthCheckResult>()
            .Produces<ApiHealthCheckResult[]>(statusCode: StatusCodes.Status503ServiceUnavailable)
            .WithSummary("Service health check")
            .WithDescription("Service health check");
    }
}
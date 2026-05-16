using Database.EntityFramework.Extensions;
using Observability.OpenTelemetry.Extensions;
using ProxyServerAggregator.Extensions;
using ProxyServerAggregator.Persistence;
using ProxyServerAggregator.Presentation.Extensions;
using ProxyServerAggregator.Presentation.Middleware;
using Serilog;
using Server.Kestrel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservabilityOpenTelemetry("proxy-server-aggregator");

builder.Services.AddProxyServerAggregatorModule();

builder.UseKestrelServer();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MigrateDatabase<AppDbContext>();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}

app.UseMiddleware<CorrelationLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpLogging();
app.UseRateLimiter();
app.UseRouting();
app.UseCors();

app.MapEndpoints();
app.MapGrpcServices();

app.Run();

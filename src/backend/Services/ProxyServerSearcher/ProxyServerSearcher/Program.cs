using Database.EntityFramework.Extensions;
using Observability.OpenTelemetry.Extensions;
using ProxyServerSearcher.Extensions;
using ProxyServerSearcher.Persistence;
using ProxyServerSearcher.Presentation.Extensions;
using ProxyServerSearcher.Presentation.Middleware;
using Serilog;
using Server.Kestrel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservabilityOpenTelemetry("proxy-server-searcher");

builder.Services.AddProxyServerSearcherModule();

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

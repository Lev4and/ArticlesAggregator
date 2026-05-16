using Database.EntityFramework.Extensions;
using Observability.OpenTelemetry.Extensions;
using ProxyServerTester.Extensions;
using ProxyServerTester.Persistence;
using ProxyServerTester.Presentation.Extensions;
using ProxyServerTester.Presentation.Middleware;
using Serilog;
using Server.Kestrel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservabilityOpenTelemetry("proxy-server-tester");

builder.Services.AddProxyServerTesterModule();

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
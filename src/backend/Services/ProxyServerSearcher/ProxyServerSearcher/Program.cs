using Observability.OpenTelemetry.Extensions;
using ProxyServerSearcher.Extensions;
using Serilog;
using Server.Kestrel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservabilityOpenTelemetry("proxy-server-searcher");

builder.Services.AddProxyServerSearcherModule();

builder.UseKestrelServer();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();

app.Run();

using ProxyServerSearcher.Extensions;
using Server.Kestrel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProxyServerSearcherModule();

builder.WebHost.UseKestrelServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

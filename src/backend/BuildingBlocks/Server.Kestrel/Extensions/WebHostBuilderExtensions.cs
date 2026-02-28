using System.Security.Authentication;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

using Server.Kestrel.Configurations;

namespace Server.Kestrel.Extensions;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder UseKestrelServer(this IWebHostBuilder builder, 
        IKestrelServerConfiguration? configuration = null)
    {
        configuration ??= new KestrelServerConfiguration();
        
        builder.UseKestrel(options =>
        {
            options.ConfigureHttpsDefaults(configureOptions =>
            {
                configureOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });
            
            options.ListenAnyIP(configuration.HttpPort);
            options.ListenAnyIP(configuration.GrpcPort, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
        });
        
        return builder;
    }
}
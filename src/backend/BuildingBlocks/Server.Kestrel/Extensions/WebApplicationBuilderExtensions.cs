using System.Security.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Server.Kestrel.Configurations;

namespace Server.Kestrel.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder UseKestrelServer(IKestrelServerConfiguration? serverConfiguration = null)
        {
            serverConfiguration ??= new KestrelServerConfiguration();
            
            builder.Services.AddSingleton(serverConfiguration);
            
            var serviceProvider = builder.Services.BuildServiceProvider();
            var configuration   = serviceProvider.GetRequiredService<IKestrelServerConfiguration>();
            
            builder.WebHost.UseKestrel(options =>
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
}
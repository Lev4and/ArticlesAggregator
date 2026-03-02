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
        public WebApplicationBuilder AddKestrelServerConfiguration()
        {
            builder.Services.AddSingleton<IKestrelServerConfiguration, KestrelServerConfiguration>();
        
            return builder;
        }

        public WebApplicationBuilder AddKestrelServerConfiguration<TConfiguration>()
            where TConfiguration : IKestrelServerConfiguration
        {
            builder.Services.AddSingleton(typeof(IKestrelServerConfiguration), typeof(TConfiguration));
            
            return builder;
        }

        public WebApplicationBuilder UseKestrelServer()
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
        
            var configuration = serviceProvider.GetRequiredService<IKestrelServerConfiguration>();
            
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
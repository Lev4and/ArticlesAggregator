using System.Net;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using ProxyServerSearcher.Infrastructure.ProxyServers.Sources;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddProxyServers()
        {
            typeof(IProxyServerSourceRegistrator).Assembly.GetTypes()
                .Where(type => type is { IsClass: true, IsAbstract: false } && 
                    type.HasInterface(typeof(IProxyServerSourceRegistrator)))
                .ForEach(registratorType =>
                {
                    services.AddSingleton(typeof(IProxyServerSourceRegistrator), registratorType);
                });
            
            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetServices<IProxyServerSourceRegistrator>()
                .ForEach(registrator =>
                {
                    registrator.Register(services);
                });
            
            return services;
        }

        public IServiceCollection AddProxyServerSourceClient(string sourceName, string sourceUrl)
        {
            services
                .AddHttpClient(sourceName, client =>
                {
                    client.BaseAddress = new Uri(sourceUrl);
                    client.Timeout     = TimeSpan.FromSeconds(30);
                    
                    client.DefaultRequestHeaders.UseDefaultHeaders();
                })
                .ConfigurePrimaryHttpMessageHandler(() => 
                    new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        AutomaticDecompression                    = DecompressionMethods.All,
                        AllowAutoRedirect                         = true
                    });
            
            return services;
        }
    }
}
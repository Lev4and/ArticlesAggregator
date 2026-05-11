using System.Net;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Missions.Hosting.Extensions;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.UseCases.ProxyServers.Missions;
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
            
            services.AddScoped<IProxyServerSourceService, ProxyServerSourceService>();
            services.AddScoped<IProxyServerService, ProxyServerService>();

            services.AddMission<ProxyServerSearchPlanMission>(
                TimeSpan.FromSeconds(5), TimeSpan.FromHours(1));

            services.AddMission<ProxyServerSearchStoredTaskMission>(
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            
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
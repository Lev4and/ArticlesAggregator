using Microsoft.Extensions.DependencyInjection;
using Missions.Hosting.Extensions;
using ProxyServerTester.Application.Abstracts.ProxyServers;
using ProxyServerTester.Application.UseCases.ProxyServers.Missions;

namespace ProxyServerTester.Infrastructure.ProxyServers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxyServers(this IServiceCollection services)
    {
        services.AddScoped<IProxyServerTestRequestService, ProxyServerTestRequestService>();
        services.AddScoped<IProxyServerTester, ProxyServerTester>();
        
        services.AddMission<ProxyServerTestStoredTaskMission>(
            TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        
        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Extensions;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.ProxyVerity;

public class ProxyVerityProxyServerSourceRegistrator : IProxyServerSourceRegistrator
{
    public void Register(IServiceCollection services)
    {
        services.AddProxyServerSourceClient(
            sourceName: ProxyServerSourceConstants.ProxyVerity,
            sourceUrl: "https://proxyverity.com/");
        
        services.AddScoped<ProxyVerityClient>();

        services.AddKeyedScoped<IProxyServerSource, ProxyVerityProxyServerSource>(
            serviceKey: ProxyServerSourceConstants.ProxyVerity);
    }
}
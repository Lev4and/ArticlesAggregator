using Microsoft.Extensions.DependencyInjection;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Extensions;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.ProxyMania;

public class ProxyManiaProxyServerSourceRegistrator : IProxyServerSourceRegistrator
{
    public void Register(IServiceCollection services)
    {
        services.AddProxyServerSourceClient(
            sourceName: ProxyServerSourceConstants.ProxyMania,
            sourceUrl: "https://proxymania.su/");
        
        services.AddScoped<ProxyManiaClient>();

        services.AddKeyedScoped<IProxyServerSource, ProxyManiaProxyServerSource>(
            serviceKey: ProxyServerSourceConstants.ProxyMania);
    }
}
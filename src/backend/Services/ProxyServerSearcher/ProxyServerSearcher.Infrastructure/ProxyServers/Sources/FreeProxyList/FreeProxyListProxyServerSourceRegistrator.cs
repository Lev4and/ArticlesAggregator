using Microsoft.Extensions.DependencyInjection;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Extensions;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList;

public class FreeProxyListProxyServerSourceRegistrator : IProxyServerSourceRegistrator
{
    public void Register(IServiceCollection services)
    {
        services.AddProxyServerSourceClient(
            sourceName: ProxyServerSourceConstants.FreeProxyList,
            sourceUrl: "https://cdn.jsdelivr.net/gh/proxifly/free-proxy-list@main/");
        
        services.AddScoped<FreeProxyListClient>();

        services.AddKeyedScoped<IProxyServerSource, FreeProxyListProxyServerSource>(
            serviceKey: ProxyServerSourceConstants.FreeProxyList);
    }
}
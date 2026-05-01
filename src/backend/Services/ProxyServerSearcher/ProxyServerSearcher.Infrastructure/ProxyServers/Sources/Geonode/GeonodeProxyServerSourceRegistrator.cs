using Microsoft.Extensions.DependencyInjection;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Extensions;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode;

public class GeonodeProxyServerSourceRegistrator : IProxyServerSourceRegistrator
{
    public void Register(IServiceCollection services)
    {
        services.AddProxyServerSourceClient(
            sourceName: ProxyServerSourceConstants.Geonode,
            sourceUrl: "https://proxylist.geonode.com/api/");
        
        services.AddScoped<GeonodeClient>();

        services.AddKeyedScoped<IProxyServerSource, GeonodeProxyServerSource>(
            serviceKey: ProxyServerSourceConstants.Geonode);
    }
}
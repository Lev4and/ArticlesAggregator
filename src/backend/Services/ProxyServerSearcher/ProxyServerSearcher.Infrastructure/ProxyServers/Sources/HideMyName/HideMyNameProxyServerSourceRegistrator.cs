using Microsoft.Extensions.DependencyInjection;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Extensions;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.HideMyName;

public class HideMyNameProxyServerSourceRegistrator : IProxyServerSourceRegistrator
{
    public void Register(IServiceCollection services)
    {
        services.AddProxyServerSourceClient(
            sourceName: ProxyServerSourceConstants.HideMyName,
            sourceUrl: "https://hide-my-name.com/");
        
        services.AddScoped<HideMyNameClient>();

        services.AddKeyedScoped<IProxyServerSource, HideMyNameProxyServerSource>(
            serviceKey: ProxyServerSourceConstants.HideMyName);
    }
}
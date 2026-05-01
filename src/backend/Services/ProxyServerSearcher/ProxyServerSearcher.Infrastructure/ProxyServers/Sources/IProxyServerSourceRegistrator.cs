using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources;

public interface IProxyServerSourceRegistrator
{
    void Register(IServiceCollection services);
}
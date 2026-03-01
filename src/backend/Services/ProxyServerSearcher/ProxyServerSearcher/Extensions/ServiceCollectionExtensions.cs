using ProxyServerSearcher.Application.Extensions;
using ProxyServerSearcher.Infrastructure.Extensions;
using ProxyServerSearcher.Persistence.Extensions;
using ProxyServerSearcher.Presentation.Extensions;

namespace ProxyServerSearcher.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxyServerSearcherModule(this IServiceCollection services)
    {
        services.AddApplication();
        services.AddPersistence();
        services.AddInfrastructure();
        services.AddPresentation();
        
        return services;
    }
}
using ProxyServerAggregator.Application.Extensions;
using ProxyServerAggregator.Infrastructure.Extensions;
using ProxyServerAggregator.Persistence.Extensions;
using ProxyServerAggregator.Presentation.Extensions;

namespace ProxyServerAggregator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxyServerAggregatorModule(this IServiceCollection services)
    {
        services.AddApplication();
        services.AddPersistence();
        services.AddInfrastructure();
        services.AddPresentation();
        
        return services;
    }
}
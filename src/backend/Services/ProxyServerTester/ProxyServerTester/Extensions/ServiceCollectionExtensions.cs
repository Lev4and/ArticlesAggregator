using ProxyServerTester.Application.Extensions;
using ProxyServerTester.Infrastructure.Extensions;
using ProxyServerTester.Persistence.Extensions;
using ProxyServerTester.Presentation.Extensions;

namespace ProxyServerTester.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxyServerTesterModule(this IServiceCollection services)
    {
        services.AddApplication();
        services.AddPersistence();
        services.AddInfrastructure();
        services.AddPresentation();
        
        return services;
    }
}
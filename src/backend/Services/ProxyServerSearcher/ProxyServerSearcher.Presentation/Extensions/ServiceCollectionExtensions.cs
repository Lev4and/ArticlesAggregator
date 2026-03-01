using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerSearcher.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        
        return services;
    }
}
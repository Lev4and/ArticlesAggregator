using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerSearcher.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
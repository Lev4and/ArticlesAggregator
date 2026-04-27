using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerSearcher.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.All;
        });
        
        services.AddOpenApi();
        
        return services;
    }
}
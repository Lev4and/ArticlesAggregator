using System.Reflection;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DomainEvents.Abstracts.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services, 
        params Assembly[] assemblies)
    {
        var assembliesTypes = assemblies.GetTypes().ToArray();
        
        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && 
                type.HasInterface(typeof(IDomainEventHandler<>)))
            .ForEach(domainEventHandlerType =>
            {
                domainEventHandlerType.GetInterfaces()
                    .Where(interfaceType => interfaceType.IsGenericType && 
                        interfaceType.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                    .ForEach(domainEventHandlerInterfaceType =>
                    {
                        services.TryAddScoped(domainEventHandlerInterfaceType, domainEventHandlerType);
                    });
            });
        
        return services;
    }
}
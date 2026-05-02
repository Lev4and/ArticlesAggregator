using System.Reflection;
using DomainEvents.Abstracts;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoredTasks.Abstracts;

namespace DomainEvents.Database.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEvents(this IServiceCollection services, params Assembly[] assemblies)
    {
        var assembliesTypes = assemblies.GetTypes().ToArray();
        
        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && type.HasInterface(typeof(IDomainEventHandler<>)))
            .ForEach(domainEventHandlerType =>
            {
                domainEventHandlerType
                    .GetGenericInterfaces(typeof(IDomainEventHandler<>))
                    .ForEach(handlerInterfaceType =>
                    {
                        services.TryAddScoped(handlerInterfaceType, domainEventHandlerType);
                    });
            });
        
        return services;
    }
}
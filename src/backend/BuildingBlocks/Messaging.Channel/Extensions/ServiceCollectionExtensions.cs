using System.Reflection;
using Extensions;
using Messaging.Abstracts;
using Messaging.Channel.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Messaging.Channel.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChannelMessaging(this IServiceCollection services, params Assembly[] assemblies)
    {
        var assembliesTypes = assemblies.GetTypes().ToArray();
        
        services.AddSingleton<ChannelQueueDictionary>();
        services.AddScoped<ChannelExchange>();
        
        services.AddScoped<IMessageProducer, ChannelProducer>();
        
        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && type.HasInterface(typeof(IMessageHandler<>)))
            .ForEach(messageHandlerType =>
            {
                messageHandlerType
                    .GetGenericInterfaces(typeof(IMessageHandler<>))
                    .ForEach(handlerInterfaceType =>
                    {
                        services.TryAddScoped(handlerInterfaceType, messageHandlerType);
                    });
            });
        
        return services;
    }

    public static IServiceCollection AddChannelConsumer(this IServiceCollection services, 
        IChannelConsumerConfiguration configuration)
    {
        var consumerConfigurationKey = $"ChannelConsumerConfiguration-{Guid.NewGuid().ToString()}";
        var consumerWorkerKey        = $"ChannelConsumerWorker-{Guid.NewGuid().ToString()}";
            
        services.AddKeyedSingleton(consumerConfigurationKey, configuration);

        services.AddKeyedScoped(consumerWorkerKey,
            (sp, key) => 
                ActivatorUtilities.CreateInstance<ChannelConsumerWorker>(
                    sp,
                    sp.GetRequiredKeyedService<IChannelConsumerConfiguration>(consumerConfigurationKey)));
        
        services.AddHostedService<ChannelConsumerWorkerService>(
            sp => 
                ActivatorUtilities.CreateInstance<ChannelConsumerWorkerService>(
                    sp, 
                    consumerWorkerKey));
        
        return services;
    }
}
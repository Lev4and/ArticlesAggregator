using System.Reflection;
using Messaging.Abstracts;
using Messaging.Abstracts.Extensions;
using Messaging.Channel.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Channel.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChannelMessaging(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<ChannelQueueDictionary>();
        
        services.AddScoped<ChannelExchange>();
        
        services.AddScoped<IMessageProducer, ChannelProducer>();

        services.AddMessageHandlers(assemblies);
        
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
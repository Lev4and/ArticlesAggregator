using Messaging.Outbox.Abstracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Missions.Hosting.Extensions;
using StoredTasks.Abstracts;

namespace Messaging.Outbox.Handling.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingOutboxHandling(this IServiceCollection services)
    {
        services.TryAddScoped<IStoredTaskHandler<OutboxMessage>, OutboxMessageTaskHandler>();

        services.AddMission<OutboxMessageTaskMission>(
            TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        
        return services;
    }
}
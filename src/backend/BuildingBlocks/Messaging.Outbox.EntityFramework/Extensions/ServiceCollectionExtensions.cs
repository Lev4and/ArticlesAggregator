using Database.EntityFramework;
using Messaging.Outbox.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoredTasks.Database.Abstracts;

namespace Messaging.Outbox.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEntityFrameworkOutboxMessageRepository<TDbContext>()
            where TDbContext : BaseDbContext
        {
            services.TryAddScoped<IOutboxMessageRepository, OutboxMessageRepository<TDbContext>>();
            
            services.TryAddScoped<IStoredTaskRepository<OutboxMessage>, OutboxMessageRepository<TDbContext>>();

            return services;
        }
    }
}
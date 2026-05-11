using Database.EntityFramework;
using Messaging.Inbox.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Messaging.Inbox.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEntityFrameworkInboxMessageRepository<TDbContext>()
            where TDbContext : BaseDbContext
        {
            services.TryAddScoped<IInboxMessageRepository, InboxMessageRepository<TDbContext>>();
        
            return services;
        }
    }
}
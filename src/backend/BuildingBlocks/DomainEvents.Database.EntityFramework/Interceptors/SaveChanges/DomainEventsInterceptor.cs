using System.Text.Json;
using DomainEvents.Database.Abstracts;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DomainEvents.Database.EntityFramework.Interceptors.SaveChanges;

public class DomainEventsInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var domainEvents = dbContext.ChangeTracker.Entries<IHasDomainEvents>()
            .SelectMany(entry => entry.Entity.GetDomainEvents()
                .Select(@event => new DomainEvent
                {
                    Id      = Guid.NewGuid(),
                    Type    = @event.GetType().Name,
                    Content = JsonSerializer.SerializeToDocument(@event),
                }));
        
        dbContext.Set<DomainEvent>().AddRange(domainEvents);
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    } 
}
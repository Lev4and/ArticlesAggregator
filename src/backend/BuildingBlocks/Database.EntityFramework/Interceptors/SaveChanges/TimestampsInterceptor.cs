using Contracts;

using Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Database.EntityFramework.Interceptors.SaveChanges;

public class TimestampsInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, 
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var utcNow = DateTime.UtcNow;
        
        dbContext.ChangeTracker.Entries<IHasTimestamps>()
            .Where(entry => entry.State == EntityState.Added)
            .ForEach(entry =>
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
            });
        
        dbContext.ChangeTracker.Entries<IHasTimestamps>()
            .Where(entry => entry.State is EntityState.Modified or EntityState.Deleted)
            .ForEach(entry =>
            {
                entry.Entity.UpdatedAt = utcNow;
            });
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
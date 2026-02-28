using Contracts;

using Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Database.EntityFramework.Interceptors.SaveChanges;

public class SoftDeleteInterceptor : SaveChangesInterceptor
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

        dbContext.ChangeTracker.Entries<IHasSoftDelete>()
            .Where(entry => entry.State == EntityState.Deleted)
            .ForEach(entry =>
            {
                entry.State = EntityState.Modified;

                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = utcNow;
            });
        
        dbContext.ChangeTracker.Entries<IHasSoftDelete>()
            .Where(entry => entry is { State: EntityState.Modified, Entity.IsDeleted: true })
            .ForEach(entry =>
            {
                entry.Entity.IsDeleted = false;
                entry.Entity.DeletedAt = null;
            });

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
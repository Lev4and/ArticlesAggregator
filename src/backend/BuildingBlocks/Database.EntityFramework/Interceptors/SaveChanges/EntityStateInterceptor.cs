using Contracts;

using Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Database.EntityFramework.Interceptors.SaveChanges;

public class EntityStateInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, 
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        
        dbContext.ChangeTracker.Entries<IHasEntityState>()
            .Where(entry => entry.State == EntityState.Added)
            .ForEach(entry =>
            {
                entry.Entity.EntityState = Primitives.EntityState.Created;
            });
        
        dbContext.ChangeTracker.Entries<IHasEntityState>()
            .Where(entry => entry.State == EntityState.Modified)
            .ForEach(entry =>
            {
                entry.Entity.EntityState = Primitives.EntityState.Updated;
            });
        
        dbContext.ChangeTracker.Entries<IHasEntityState>()
            .Where(entry => entry.State == EntityState.Deleted)
            .ForEach(entry =>
            {
                entry.Entity.EntityState = Primitives.EntityState.Removed;
            });
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
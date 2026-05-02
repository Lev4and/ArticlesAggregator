using Database.EntityFramework;
using Database.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using StoredTasks.Abstracts;
using StoredTasks.Database.Abstracts;
using EntityState = Primitives.EntityState;

namespace StoredTasks.Database.EntityFramework.Repositories;

public abstract class StoredTaskRepository<TDbContext, TStoredTask> : 
    EntityFrameworkRepository<TDbContext, TStoredTask, Guid>, IStoredTaskRepository<TStoredTask>
    where TDbContext : BaseDbContext where TStoredTask : StoredTask
{
    protected StoredTaskRepository(TDbContext dbContext) : base(dbContext)
    {
        
    }
    
    public async Task<int> MarkExpiredTasksAsFailedAsync(CancellationToken ct = default)
    {
        return await DbContext.Set<TStoredTask>()
            .Where(task => task.State == StoredTaskState.Processing && task.AttemptDeadline < DateTime.UtcNow &&
                (task.Deadline < DateTime.UtcNow || task.AttemptsRemaining == 1))
            .ExecuteUpdateAsync(
                task => task
                    .SetProperty(t => t.State, StoredTaskState.Failed)
                    .SetProperty(t => t.UpdatedAt, t => DateTime.UtcNow), 
                ct);
    }

    public async Task<int> CaptureTasksAsync(string workerId, DateTime attemptDeadline, 
        int limit = 10, CancellationToken ct = default)
    {
        return await DbContext.Set<TStoredTask>()
            .Where(task => task.State == StoredTaskState.Created || 
                (task.State == StoredTaskState.Processing && task.AttemptDeadline < DateTime.UtcNow && 
                    (task.Deadline > DateTime.UtcNow || task.AttemptsRemaining > 1)))
            .Take(limit)
            .ExecuteUpdateAsync(
                task => task
                    .SetProperty(t => t.State, StoredTaskState.Processing)
                    .SetProperty(t => t.WorkerId, workerId)
                    .SetProperty(t => t.AttemptDeadline, attemptDeadline)
                    .SetProperty(t => t.AttemptsRemaining, t => t.AttemptsRemaining - 1)
                    .SetProperty(t => t.Attempts, t => t.Attempts + 1)
                    .SetProperty(t => t.EntityState, EntityState.Updated)
                    .SetProperty(t => t.UpdatedAt, t => DateTime.UtcNow), 
                ct);
    }

    public async Task<IReadOnlyCollection<TStoredTask>> GetCapturedTasksAsync(string workerId,
        int limit = 10, CancellationToken ct = default)
    {
        return await DbContext.Set<TStoredTask>().AsNoTracking()
            .Where(task => task.State == StoredTaskState.Processing && task.WorkerId == workerId)
                .Take(limit)
                    .ToListAsync(ct);
    }

    public async Task MarkTaskAsCompletedAsync(Guid taskId, CancellationToken ct = default)
    {
        await DbContext.Set<TStoredTask>().AsNoTracking()
            .Where(task => task.Id == taskId)
            .ExecuteUpdateAsync(
                task => task
                    .SetProperty(t => t.State, StoredTaskState.Completed)
                    .SetProperty(t => t.UpdatedAt, DateTime.UtcNow),
                ct);
    }

    public async Task MarkTaskAsFailedAsync(Guid taskId, CancellationToken ct = default)
    {
        await DbContext.Set<TStoredTask>().AsNoTracking()
            .Where(task => task.Id == taskId)
            .ExecuteUpdateAsync(
                task => task
                    .SetProperty(t => t.State, StoredTaskState.Failed)
                    .SetProperty(t => t.UpdatedAt, DateTime.UtcNow),
                ct);
    }
}
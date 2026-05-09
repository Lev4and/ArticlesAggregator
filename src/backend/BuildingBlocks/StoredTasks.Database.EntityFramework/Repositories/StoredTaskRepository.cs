using Database.EntityFramework;
using Database.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using StoredTasks.Abstracts;
using StoredTasks.Database.Abstracts;
using EntityState = Primitives.EntityState;

namespace StoredTasks.Database.EntityFramework.Repositories;

public abstract class StoredTaskRepository<TDbContext, TStoredTask> : 
    EntityFrameworkRepository<TDbContext, TStoredTask, Guid>, IStoredTaskRepository<TStoredTask>
    where TDbContext : BaseDbContext where TStoredTask : StoredTask
{
    protected StoredTaskRepository(
        ITracer<EntityFrameworkRepository<TDbContext, TStoredTask, Guid>> tracer, 
        ILogger<EntityFrameworkRepository<TDbContext, TStoredTask, Guid>> logger, 
        TDbContext dbContext) : 
        base(tracer, logger, dbContext)
    {
        
    }

    public async Task<int> MarkExpiredTasksAsFailedAsync(CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Mark expired stored tasks as failed in db");
        
        Logger.LogInformation("Marked expired stored tasks as failed in db");
        
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
        using var operation = Tracer.StartOperation("Capture stored tasks in db");
        
        Logger.LogInformation("Captured stored tasks in db");
        
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
        using var operation = Tracer.StartOperation("Get captured stored tasks from db");
        
        Logger.LogInformation("Get captured stored tasks from db");
        
        return await DbContext.Set<TStoredTask>().AsNoTracking()
            .Where(task => task.State == StoredTaskState.Processing && task.WorkerId == workerId)
                .Take(limit)
                    .ToListAsync(ct);
    }

    public async Task MarkTaskAsCompletedAsync(Guid taskId, CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Mark stored task as completed in db");
        
        Logger.LogInformation("Mark stored task as completed in db Id: {StoredTaskId}", taskId);
        
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
        using var operation = Tracer.StartOperation("Mark stored task as failed in db");
        
        Logger.LogInformation("Mark stored task as failed in db Id: {StoredTaskId}", taskId);
        
        await DbContext.Set<TStoredTask>().AsNoTracking()
            .Where(task => task.Id == taskId)
            .ExecuteUpdateAsync(
                task => task
                    .SetProperty(t => t.State, StoredTaskState.Failed)
                    .SetProperty(t => t.UpdatedAt, DateTime.UtcNow),
                ct);
    }
}
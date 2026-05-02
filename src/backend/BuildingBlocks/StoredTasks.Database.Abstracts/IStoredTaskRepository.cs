using Database.Abstracts;

namespace StoredTasks.Database.Abstracts;

public interface IStoredTaskRepository<TStoredTask> : IRepository<TStoredTask, Guid>
    where TStoredTask : StoredTask
{
    Task<int> CaptureTasksAsync(string workerId, DateTime attemptDeadline, 
        int limit = 10, CancellationToken ct = default);

    Task<IReadOnlyCollection<TStoredTask>> GetCapturedTasksAsync(string workerId, 
        int limit = 10, CancellationToken ct = default);
    
    Task<int> MarkExpiredTasksAsFailedAsync(CancellationToken ct = default);
    
    Task MarkTaskAsCompletedAsync(Guid taskId, CancellationToken ct = default);
    
    Task MarkTaskAsFailedAsync(Guid taskId, CancellationToken ct = default);
}
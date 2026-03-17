using Database.Abstracts;

namespace StoredTasks.Database.Abstracts;

public interface IStoredTaskRepository<TStoredTask> : IRepository<TStoredTask, Guid>
    where TStoredTask : StoredTask
{
    Task<TStoredTask[]> GetTasksForExecuteAsync(int limit, string workerId, 
        CancellationToken cancellationToken = default);

    Task MarkTaskAsCompletedAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task MarkTaskAsFailedAsync(Guid taskId, CancellationToken cancellationToken = default);
    
    Task MarkExpiredTasksAsFailedAsync(CancellationToken cancellationToken = default);
}
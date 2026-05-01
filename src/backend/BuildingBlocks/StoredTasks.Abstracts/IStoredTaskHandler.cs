using Result;

namespace StoredTasks.Abstracts;

public interface IStoredTaskHandler<in TStoredTask> : IStoredTaskHandler
    where TStoredTask : IStoredTask
{
    Task<AppResult> HandleAsync(TStoredTask storedTask, CancellationToken ct = default);

    Task<AppResult> IStoredTaskHandler.HandleAsync(IStoredTask storedTask, CancellationToken ct)
    {
        return HandleAsync((TStoredTask)storedTask, ct);
    }
}

public interface IStoredTaskHandler
{
    Task<AppResult> HandleAsync(IStoredTask storedTask, CancellationToken ct = default);
}
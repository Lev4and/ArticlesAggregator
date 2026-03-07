using Result;

namespace StoredTasks.Abstracts;

public interface IStoredTaskHandler<TStoredTask> : IStoredTaskHandler
    where TStoredTask : IStoredTask
{
    Task<AppResult> HandleAsync(TStoredTask storedTask, CancellationToken cancellationToken = default);

    Task<AppResult> IStoredTaskHandler.HandleAsync(IStoredTask storedTask, CancellationToken cancellationToken)
    {
        return HandleAsync((TStoredTask)storedTask, cancellationToken);
    }
}

public interface IStoredTaskHandler
{
    Task<AppResult> HandleAsync(IStoredTask storedTask, CancellationToken cancellationToken = default);
}
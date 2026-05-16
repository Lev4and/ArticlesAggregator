using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Missions.Abstracts;
using Observability.Abstracts;
using StoredTasks.Abstracts;
using StoredTasks.Database.Abstracts;

namespace StoredTasks.Database.Missions;

public abstract class StoredTaskMission<TStoredTask> : IMission
    where TStoredTask : StoredTask
{
    protected readonly ITracer<StoredTaskMission<TStoredTask>> Tracer;
    protected readonly ILogger<StoredTaskMission<TStoredTask>> Logger;
    protected readonly IServiceScopeFactory                    ServiceScopeFactory;
    
    protected virtual int TaskLimit => 10;

    protected virtual TimeSpan AttemptDuration => TimeSpan.FromMinutes(1);
    
    protected virtual int WorkerCount => 1;

    protected virtual TimeSpan? IntervalDelay => TimeSpan.FromSeconds(5);

    public StoredTaskMission(
        ITracer<StoredTaskMission<TStoredTask>> tracer,
        ILogger<StoredTaskMission<TStoredTask>> logger,
        IServiceScopeFactory                    serviceScopeFactory)
    {
        Tracer              = tracer;
        Logger              = logger;
        ServiceScopeFactory = serviceScopeFactory;
    }
    
    public virtual async Task RunAsync(CancellationToken ct = default)
    {
        Logger.LogInformation("Stored task mission started Type: {StoredTaskType}", typeof(TStoredTask).Name);

        var workers = Enumerable.Range(1, WorkerCount)
            .Select(_ => RunWorkerAsync(Guid.NewGuid().ToString(), ct));
        
        await Task.WhenAll(workers);
        
        Logger.LogInformation("Stored task mission completed Type: {StoredTaskType}", typeof(TStoredTask).Name);
    }

    protected virtual async Task RunWorkerAsync(string workerId, CancellationToken ct = default)
    {
        Logger.LogInformation("Stored task mission started Type: {StoredTaskType} WorkerId: {WorkerId}", 
            typeof(TStoredTask).Name, workerId);
        
        while (!ct.IsCancellationRequested)
        {
            await using var scope      = ServiceScopeFactory.CreateAsyncScope();
            await using var repository = scope.ServiceProvider.GetRequiredService<IStoredTaskRepository<TStoredTask>>();
            
            await repository.MarkExpiredTasksAsFailedAsync(ct);
            
            await repository.CaptureTasksAsync(workerId, DateTime.UtcNow.Add(AttemptDuration), TaskLimit, ct);
            
            var capturedTasks = await repository.GetCapturedTasksAsync(workerId, TaskLimit, ct);
            if (capturedTasks.Count == 0)
            {
                if (IntervalDelay is not null)
                {
                    await Task.Delay(IntervalDelay.Value, ct);
                    
                    continue;
                }
            }
            
            foreach (var capturedTask in capturedTasks)
            {
                using var storedTaskHandleOperation = Tracer.StartOperation("Stored task handle");
                
                Logger.LogInformation("Stored task handle Id: {StoredTaskId}", capturedTask.Id);
                
                var storedTaskHandler = (IStoredTaskHandler?)scope.ServiceProvider.GetService(typeof(IStoredTaskHandler<TStoredTask>));
                if (storedTaskHandler is null)
                {
                    Logger.LogError("Stored task handler not found Id: {StoredTaskId} Type: {StoredTaskType}",
                        capturedTask.Id, typeof(TStoredTask).Name);
                    
                    await repository.MarkTaskAsFailedAsync(capturedTask.Id, ct);
                    
                    continue;
                }

                try
                {
                    var handleResult = await storedTaskHandler.HandleAsync(capturedTask, ct);
                    if (handleResult.IsSuccess)
                    {
                        Logger.LogInformation("Stored task handle successfully completed Id: {StoredTaskId}",
                            capturedTask.Id);
                        
                        await repository.MarkTaskAsCompletedAsync(capturedTask.Id, ct);
                    }
                    else
                    {
                        var error = handleResult.Errors.First();

                        Logger.LogWarning(
                            "Stored task handle completed with errors Id: {StoredTaskId} ErrorType: {ErrorType} ErrorMessage: {ErrorMessage}",
                                capturedTask.Id, error.Type, error.Message);
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, "Stored task handle failed Id: {StoredTaskId}", capturedTask.Id);
                }
            }
        }
        
        Logger.LogInformation("Stored task mission completed Type: {StoredTaskType} WorkerId: {WorkerId}", 
            typeof(TStoredTask).Name, workerId);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        
        return ValueTask.CompletedTask;
    }
}
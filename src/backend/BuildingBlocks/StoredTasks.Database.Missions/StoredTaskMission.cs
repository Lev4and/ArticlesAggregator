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
    protected readonly IStoredTaskRepository<TStoredTask>      Repository;
    protected readonly IServiceProvider                        ServiceProvider;
    
    protected virtual int TaskLimit => 10;
    
    protected virtual int ConcurrentCount => 1;
    
    protected virtual TimeSpan AttemptDuration => TimeSpan.FromMinutes(1);
    
    public StoredTaskMission(
        ITracer<StoredTaskMission<TStoredTask>> tracer,
        ILogger<StoredTaskMission<TStoredTask>> logger,
        IStoredTaskRepository<TStoredTask>      repository, 
        IServiceProvider                        serviceProvider)
    {
        Tracer          = tracer;
        Logger          = logger;
        Repository      = repository;
        ServiceProvider = serviceProvider;
    }
    
    public virtual async Task RunAsync(CancellationToken ct = default)
    {
        Logger.LogInformation("Stored task mission started Type: {StoredTaskType}", typeof(TStoredTask).Name);

        var workers = Enumerable.Range(1, ConcurrentCount)
            .Select(_ => RunWorkerAsync(Guid.NewGuid().ToString(), ct))
                .ToArray();
        
        await Task.WhenAll(workers);
        
        Logger.LogInformation("Stored task mission completed Type: {StoredTaskType}", typeof(TStoredTask).Name);
    }

    protected virtual async Task RunWorkerAsync(string workerId, CancellationToken ct = default)
    {
        Logger.LogInformation("Stored task mission started Type: {StoredTaskType} WorkerId: {WorkerId}", 
            typeof(TStoredTask).Name, workerId);
        
        while (!ct.IsCancellationRequested)
        {
            await Repository.MarkExpiredTasksAsFailedAsync(ct);
            
            await Repository.CaptureTasksAsync(workerId, DateTime.UtcNow.Add(AttemptDuration), TaskLimit, ct);
            
            var capturedTasks = await Repository.GetCapturedTasksAsync(workerId, TaskLimit, ct);
            
            foreach (var capturedTask in capturedTasks)
            {
                Logger.LogInformation("Stored task handle Id: {StoredTaskId}", capturedTask.Id);
                
                var storedTaskHandler = (IStoredTaskHandler?)ServiceProvider.GetService(typeof(IStoredTaskHandler<TStoredTask>));
                if (storedTaskHandler is null)
                {
                    Logger.LogError("Stored task handler not found Id: {StoredTaskId} Type: {StoredTaskType}",
                        capturedTask.Id, typeof(TStoredTask).Name);
                    
                    await Repository.MarkTaskAsFailedAsync(capturedTask.Id, ct);
                    
                    continue;
                }

                try
                {
                    var handleResult = await storedTaskHandler.HandleAsync(capturedTask, ct);
                    if (handleResult.IsSuccess)
                    {
                        Logger.LogInformation("Stored task handle successfully completed Id: {StoredTaskId}",
                            capturedTask.Id);
                        
                        await Repository.MarkTaskAsCompletedAsync(capturedTask.Id, ct);
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
        Repository.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await Repository.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}
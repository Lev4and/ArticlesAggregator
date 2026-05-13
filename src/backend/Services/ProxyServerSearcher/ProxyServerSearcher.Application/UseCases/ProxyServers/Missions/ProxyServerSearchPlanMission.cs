using Database.Abstracts;
using Extensions;
using Microsoft.Extensions.Logging;
using Missions.Abstracts;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Domain.Entities;
using StoredTasks.Database.Abstracts;

namespace ProxyServerSearcher.Application.UseCases.ProxyServers.Missions;

public class ProxyServerSearchPlanMission : IMission
{
    private readonly ITracer<ProxyServerSearchPlanMission> _tracer;
    private readonly ILogger<ProxyServerSearchPlanMission> _logger;
    private readonly IProxyServerSourceService _proxyServerSourceService;
    private readonly IStoredTaskRepository<ProxyServerSearchStoredTask> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProxyServerSearchPlanMission(
        ITracer<ProxyServerSearchPlanMission> tracer, 
        ILogger<ProxyServerSearchPlanMission> logger,
        IProxyServerSourceService proxyServerSourceService,
        IStoredTaskRepository<ProxyServerSearchStoredTask> repository,
        IUnitOfWork unitOfWork)
    {
        _tracer = tracer;
        _logger = logger;
        _proxyServerSourceService = proxyServerSourceService;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task RunAsync(CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server search plan mission");
        
        _logger.LogInformation("Proxy server search plan mission started");
        
        var listResult = await _proxyServerSourceService.GetListAsync(ct);
        if (listResult.IsFailure)
        {
            var error = listResult.Errors.First();
            
            _logger.LogError(
                "Proxy server search plan mission failed ErrorType: {ErrorType} ErrorMessage: {ErrorMessage}",
                    error.Type, error.Message);
            
            return;
        }
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var storedTasks =
                listResult.Result!.Select(sourceName => 
                    new ProxyServerSearchStoredTask
                    {
                        SourceName = sourceName,
                        PlannedAt  = DateTime.UtcNow.RoundDown(TimeSpan.FromHours(1)),
                    });
            
            _repository.AddRange(storedTasks);

            await _unitOfWork.SaveChangesAsync(ct);
            
            await transaction.CommitAsync(ct);
            
            _logger.LogInformation("Proxy server search plan mission finished");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(ct);
            
            _logger.LogError(exception, "Proxy server search plan mission failed");
        }
    }

    public void Dispose()
    {
        _repository.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _repository.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}
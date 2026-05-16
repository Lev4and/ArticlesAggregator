using Database.Abstracts;
using Messaging.Messages.ProxyServerEvents.Models;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Application.Abstracts.ProxyServers;
using ProxyServerAggregator.Application.Dtos.ProxyServers;
using ProxyServerAggregator.Domain.Repositories;
using Result;

namespace ProxyServerAggregator.Infrastructure.ProxyServers;

public class ProxyServerTestRequestService : IProxyServerTestRequestService
{
    private readonly ITracer<ProxyServerTestRequestService> _tracer;
    private readonly ILogger<ProxyServerTestRequestService> _logger;
    private readonly IProxyServerTestRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public ProxyServerTestRequestService(
        ITracer<ProxyServerTestRequestService> tracer, 
        ILogger<ProxyServerTestRequestService> logger, 
        IProxyServerTestRequestRepository repository, 
        IUnitOfWork unitOfWork)
    {
        _tracer = tracer;
        _logger = logger;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<AppResult> UpdateAsync(ProxyServerTestRequestResult requestResult, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Update proxy server test request");

        _logger.LogInformation(
            "Update proxy server test request Id: {ProxyServerTestRequestId}",
                requestResult.RequestId);

        if (requestResult.Status is ProxyServerTestRequestStatus.Created)
        {
            return AppResult.Failure(AppErrorType.Validation, "Invalid request status");
        }

        var testRequest = await _repository.FirstOrDefaultAsync(requestResult.RequestId, ct);
        if (testRequest is null)
        {
            return AppResult.Failure(AppErrorType.NotFound, "Proxy server test request not found");
        }

        if (testRequest.Status is not ProxyServerTestRequestStatus.Created)
        {
            return AppResult.Failure(AppErrorType.Validation, "Proxy server test request already completed");
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            testRequest.Status       = requestResult.Status;
            testRequest.RequestTime  = requestResult.RequestTime;
            testRequest.ResponseTime = requestResult.ResponseTime;
            testRequest.ErrorMessage = requestResult.ErrorMessage;
            
            _repository.Update(testRequest);
            
            await _unitOfWork.SaveChangesAsync(ct);
            
            await transaction.CommitAsync(ct);
            
            return AppResult.Success();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(ct);
            
            _logger.LogError(exception, "Failed to update proxy server test request");
            
            return AppResult.Failure(AppErrorType.Failed, "Failed to update proxy server test request");
        }
    }
}
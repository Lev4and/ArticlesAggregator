using Database.Abstracts;
using Messaging.Messages.ProxyServerEvents;
using Messaging.Messages.ProxyServerEvents.Models;
using Messaging.Outbox.Abstracts;
using Messaging.Outbox.Abstracts.Extensions;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerTester.Application.Abstracts.ProxyServers;
using ProxyServerTester.Application.Dtos.ProxyServers;
using ProxyServerTester.Domain.Dtos.ProxyServers;
using ProxyServerTester.Domain.Entities;
using ProxyServerTester.Domain.Repositories;
using Result;
using StoredTasks.Database.Abstracts;

namespace ProxyServerTester.Infrastructure.ProxyServers;

public class ProxyServerTestRequestService : IProxyServerTestRequestService
{
    private readonly ITracer<ProxyServerTestRequestService>           _tracer;
    private readonly ILogger<ProxyServerTestRequestService>           _logger;
    private readonly IProxyServerRepository                           _proxyServerRepository;
    private readonly IProxyServerTestRequestRepository                _proxyServerTestRequestRepository;
    private readonly IStoredTaskRepository<ProxyServerTestStoredTask> _storedTaskRepository;
    private readonly IOutboxMessageRepository                         _outboxMessageRepository;
    private readonly IUnitOfWork                                      _unitOfWork;
    
    public ProxyServerTestRequestService(
        ITracer<ProxyServerTestRequestService>           tracer, 
        ILogger<ProxyServerTestRequestService>           logger, 
        IProxyServerRepository                           proxyServerRepository, 
        IProxyServerTestRequestRepository                proxyServerTestRequestRepository,
        IStoredTaskRepository<ProxyServerTestStoredTask> storedTaskRepository,
        IOutboxMessageRepository                         outboxMessageRepository, 
        IUnitOfWork                                      unitOfWork)
    {
        _tracer                           = tracer;
        _logger                           = logger;
        _proxyServerRepository            = proxyServerRepository;
        _proxyServerTestRequestRepository = proxyServerTestRequestRepository;
        _storedTaskRepository             = storedTaskRepository;
        _outboxMessageRepository          = outboxMessageRepository;
        _unitOfWork                       = unitOfWork;
    }
    
    public async Task<AppResult> CreateAsync(ProxyServerTestRequestDto model, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Create proxy server test request");
        
        _logger.LogInformation("Create proxy server test request");

        var contains = await _proxyServerTestRequestRepository.ContainsAsync(model.Id, ct);
        if (contains)
        {
            _logger.LogWarning("Proxy server test request already exists Id: {ProxyServerTestRequestId}", model.Id);

            return AppResult.Failure(AppErrorType.Conflict, "Proxy server test request already exists");
        }

        var proxyServer = await _proxyServerRepository.FirstOrDefaultAsync(model.ProxyServer.Id, ct);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            if (proxyServer is null)
            {
                proxyServer = new ProxyServer
                {
                    Id                = model.ProxyServer.Id,
                    NormalizedName    = model.ProxyServer.NormalizedName,
                    Protocol          = model.ProxyServer.Protocol,
                    HostnameOrAddress = model.ProxyServer.HostnameOrAddress,
                    Port              = model.ProxyServer.Port,
                    Credentials       = model.ProxyServer.Credentials
                };
                
                _proxyServerRepository.Add(proxyServer);
            }

            var testRequest = new ProxyServerTestRequest
            {
                Id            = model.Id,
                ProxyServerId = model.ProxyServer.Id,
                Task          = new ProxyServerTestStoredTask
                {
                    RequestId = model.Id
                }
            };
            
            _proxyServerTestRequestRepository.Add(testRequest);
            
            await _unitOfWork.SaveChangesAsync(ct);
            
            await transaction.CommitAsync(ct);
            
            return AppResult.Success();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(ct);
            
            _logger.LogError(exception, "Failed to create proxy server test request");

            return AppResult.Failure(AppErrorType.Failed, "Failed to create proxy server test request");
        }
    }

    public async Task<AppResult<ProxyServerTestRequestDto>> GetAsync(Guid id, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Get proxy server test request");
        
        _logger.LogInformation("Get proxy server test request Id: {ProxyServerTestRequestId}", id);
        
        var testRequest = await _proxyServerTestRequestRepository.GetAsync(id, ct);
        if (testRequest is null)
        {
            _logger.LogWarning("Proxy server test request not found Id: {ProxyServerTestRequestId}", id);

            return AppResult<ProxyServerTestRequestDto>.Failure(AppErrorType.NotFound,
                "Proxy server test request not found");
        }
        
        return AppResult<ProxyServerTestRequestDto>.Success(testRequest);
    }

    public async Task<AppResult> UpdateAsync(ProxyServerTestRequestResult requestResult, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Update proxy server test request");
        
        _logger.LogInformation("Update proxy server test request Id: {ProxyServerTestRequestId}", requestResult.Id);
        
        var testRequest = await _proxyServerTestRequestRepository.FirstOrDefaultAsync(requestResult.Id, ct);
        if (testRequest is null)
        {
            _logger.LogWarning("Proxy server test request not found Id: {ProxyServerTestRequestId}", requestResult.Id);

            return AppResult<ProxyServerTestRequestDto>.Failure(AppErrorType.NotFound,
                "Proxy server test request not found");
        }

        if (testRequest.Status is not ProxyServerTestRequestStatus.Created)
        {
            _logger.LogWarning(
                "Proxy server test request already completed Id: {ProxyServerTestRequestId}", 
                    requestResult.Id);

            return AppResult.Failure(AppErrorType.Validation, "Proxy server test request already completed");
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            testRequest.Status       = string.IsNullOrEmpty(requestResult.ErrorMessage)
                ? ProxyServerTestRequestStatus.Completed
                : ProxyServerTestRequestStatus.Failed;
            testRequest.RequestTime  = requestResult.RequestTime;
            testRequest.ResponseTime = requestResult.ResponseTime;
            testRequest.ErrorMessage = requestResult.ErrorMessage;
            
            _proxyServerTestRequestRepository.Update(testRequest);
            
            var message = new ProxyServerTestRequestStatusChanged
            {
                RequestId    = testRequest.Id,
                RequestTime  = testRequest.RequestTime,
                ResponseTime = testRequest.ResponseTime,
                ErrorMessage = testRequest.ErrorMessage
            };

            var outboxMessage = message.ToOutboxMessage();
            
            _outboxMessageRepository.Add(outboxMessage);

            await _unitOfWork.SaveChangesAsync(ct);
            
            await transaction.CommitAsync(ct);
            
            outboxMessage.Dispose();
            
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
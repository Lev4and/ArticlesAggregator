using Database.Abstracts;
using Messaging.Messages.ProxyServerEvents;
using Messaging.Messages.ProxyServerEvents.Models;
using Messaging.Outbox.Abstracts;
using Messaging.Outbox.Abstracts.Extensions;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Application.Abstracts.ProxyServers;
using ProxyServerAggregator.Domain.Dtos.ProxyServers;
using ProxyServerAggregator.Domain.Entities;
using ProxyServerAggregator.Domain.Repositories;
using Result;

namespace ProxyServerAggregator.Infrastructure.ProxyServers;

public class ProxyServerService : IProxyServerService
{
    private readonly ITracer<ProxyServerService> _tracer;
    private readonly ILogger<ProxyServerService> _logger;
    private readonly IOutboxMessageRepository    _outboxMessageRepository;
    private readonly IProxyServerRepository      _repository;
    private readonly IUnitOfWork                 _unitOfWork;
    private readonly IProxyServerCacheService    _cacheService;
    
    public ProxyServerService(
        ITracer<ProxyServerService> tracer, 
        ILogger<ProxyServerService> logger, 
        IOutboxMessageRepository    outboxMessageRepository,
        IProxyServerRepository      repository, 
        IUnitOfWork                 unitOfWork, 
        IProxyServerCacheService    cacheService)
    {
        _tracer                  = tracer;
        _logger                  = logger;
        _outboxMessageRepository = outboxMessageRepository;
        _repository              = repository;
        _unitOfWork              = unitOfWork;
        _cacheService            = cacheService;
    }
    
    public async Task<AppResult> CreateAsync(ProxyServerDto model, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Create proxy server");
        
        _logger.LogInformation("Create proxy server");
        
        var contains = await _repository.ContainsAsync(model.Id, ct);
        if (contains)
        {
            _logger.LogWarning("Proxy server already exists Id: {ProxyServerId}", model.Id);
            
            return AppResult.Failure(AppErrorType.Conflict, "Proxy server already exists");
        }
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var proxyServerTestRequest = new ProxyServerTestRequest
            {
                Id            = Guid.NewGuid(),
                ProxyServerId = model.Id,
                Status        = ProxyServerTestRequestStatus.Created
            };
            
            var proxyServer = new ProxyServer
            {
                Id                = model.Id,
                NormalizedName    = model.NormalizedName,
                Protocol          = model.Protocol,
                HostnameOrAddress = model.HostnameOrAddress,
                Port              = model.Port,
                Credentials       = model.Credentials is not null
                    ? new ProxyServerCredentials
                    {
                        Username = model.Credentials.Username,
                        Password = model.Credentials.Password
                    }
                    : null,
                TestRequests     = [proxyServerTestRequest]
            };
            
            _repository.Add(proxyServer);
            
            var message = new ProxyServerTestRequestCreated
            {
                RequestId         = proxyServerTestRequest.Id,
                ProxyServerId     = proxyServer.Id,
                NormalizedName    = proxyServer.NormalizedName,
                Protocol          = proxyServer.Protocol,
                HostnameOrAddress = proxyServer.HostnameOrAddress,
                Port              = proxyServer.Port,
                Credentials       = proxyServer.Credentials is not null
                    ? new ProxyServerCredentials
                    {
                        Username = proxyServer.Credentials.Username,
                        Password = proxyServer.Credentials.Password
                    }
                    : null,
            };

            var outboxMessage = message.ToOutboxMessage();
            
            _outboxMessageRepository.Add(outboxMessage);
            
            await _unitOfWork.SaveChangesAsync(ct);
            
            await transaction.CommitAsync(ct);
            
            outboxMessage.Dispose();

            await _cacheService.SetAsync(model, ct);
            
            return AppResult.Success();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(ct);
            
            _logger.LogError(exception, "Failed to create proxy server");
            
            return AppResult.Failure(AppErrorType.Failed, "Failed to create proxy server");
        }
    }

    public async Task<AppResult<ProxyServerDto>> GetAsync(Guid id, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Get proxy server");
        
        _logger.LogInformation("Get proxy server Id: {ProxyServerId}", id);
        
        var proxyServer = await _cacheService.GetAsync(id, ct);
        if (proxyServer is not null) return AppResult<ProxyServerDto>.Success(proxyServer);
        
        proxyServer = await _repository.GetAsync(id, ct);
        
        if (proxyServer is not null)
        {
            await _cacheService.SetAsync(proxyServer, ct);
            
            return AppResult<ProxyServerDto>.Success(proxyServer);
        }
        
        _logger.LogWarning("Proxy server not found Id: {ProxyServerId}", id);
            
        return AppResult<ProxyServerDto>.Failure(AppErrorType.NotFound, "Proxy server not found");
    }

    public async ValueTask DisposeAsync()
    {
        await _outboxMessageRepository.DisposeAsync();
        await _repository.DisposeAsync();
        await _unitOfWork.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }

    public async void Dispose()
    {
        _outboxMessageRepository.Dispose();
        _repository.Dispose();
        _unitOfWork.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
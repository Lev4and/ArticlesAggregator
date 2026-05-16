using Database.Abstracts;
using Extensions;
using Messaging.Messages.ProxyServerEvents;
using Messaging.Messages.ProxyServerEvents.Models;
using Messaging.Outbox.Abstracts;
using Messaging.Outbox.Abstracts.Extensions;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.Dtos.ProxyServers;
using ProxyServerSearcher.Domain.Entities;
using ProxyServerSearcher.Domain.Repositories;
using Result;

namespace ProxyServerSearcher.Infrastructure.ProxyServers;

public class ProxyServerService : IProxyServerService
{
    private readonly ITracer<ProxyServerService> _tracer;
    private readonly ILogger<ProxyServerService> _logger;
    private readonly IOutboxMessageRepository    _outboxMessageRepository;
    private readonly IProxyServerRepository      _repository;
    private readonly IUnitOfWork                 _unitOfWork;
    
    public ProxyServerService(
        ITracer<ProxyServerService> tracer, 
        ILogger<ProxyServerService> logger, 
        IOutboxMessageRepository    outboxMessageRepository,
        IProxyServerRepository      repository,
        IUnitOfWork                 unitOfWork)
    {
        _tracer                  = tracer;
        _logger                  = logger;
        _repository              = repository;
        _outboxMessageRepository = outboxMessageRepository;
        _unitOfWork              = unitOfWork;
    }
    
    public async Task<AppResult> CreateBatchAsync(ProxyServerDto[] models, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Create batch proxy servers");
        
        _logger.LogInformation("Create batch proxy servers");
        
        var proxyServerNames = models
            .Select(proxyServer => proxyServer.NormalizedName)
                .ToArray();
        
        var oldProxyServers = await _repository.GetExistsAsync(proxyServerNames, ct);
        if (oldProxyServers.Count > 0)
        {
            _logger.LogWarning("Some proxy servers already exist Count: {ProxyServerCount}", oldProxyServers.Count);
        }
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var newProxyServers = models
                .Where(proxyServer => !oldProxyServers.Contains(proxyServer.NormalizedName))
                .Select(proxyServer =>
                    new ProxyServer
                    {
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
                            : null
                    })
                .ToArray();
            
            _repository.AddRange(newProxyServers);

            var outboxMessages = newProxyServers
                .Select(proxyServer => new ProxyServerFoundEvent
                {
                    ProxyServerId     = proxyServer.Id,
                    NormalizedName    = proxyServer.NormalizedName,
                    Protocol          = proxyServer.Protocol,
                    HostnameOrAddress = proxyServer.HostnameOrAddress,
                    Port              = proxyServer.Port,
                    Credentials       = proxyServer.Credentials
                })
                .Select(message => message.ToOutboxMessage())
                .ToArray();
            
            _outboxMessageRepository.AddRange(outboxMessages);
            
            await _unitOfWork.SaveChangesAsync(ct);
            
            await transaction.CommitAsync(ct);
            
            outboxMessages.ForEach(message => message.Dispose());

            return AppResult.Success();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(ct);
            
            _logger.LogError(exception, "Create batch proxy server failed");
            
            return AppResult.Failure(AppErrorType.Failed, exception.Message);
        }
    }

    public void Dispose()
    {
        _outboxMessageRepository.Dispose();
        _repository.Dispose();
        _unitOfWork.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _outboxMessageRepository.DisposeAsync();
        await _repository.DisposeAsync();
        await _unitOfWork.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
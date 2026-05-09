using Database.Abstracts;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace Database.EntityFramework.Transactions;

public class EntityFrameworkTransaction : IDatabaseTransaction
{
    private readonly ITracer<BaseDbContext> _tracer;
    private readonly ILogger<BaseDbContext> _logger;
    private readonly IDbContextTransaction _transaction;
    
    public EntityFrameworkTransaction(
        ITracer<BaseDbContext> tracer,
        ILogger<BaseDbContext> logger,
        IDbContextTransaction transaction)
    {
        _tracer = tracer;
        _logger = logger;
        _transaction = transaction;
    }
    
    public async Task CommitAsync(CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Commit db transaction");
        
        _logger.LogInformation("Commit db transaction");
        
        await _transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Rollback db transaction");
        
        _logger.LogInformation("Rollback db transaction");
        
        await _transaction.RollbackAsync(ct);
    }

    public void Dispose()
    {
        _transaction.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}
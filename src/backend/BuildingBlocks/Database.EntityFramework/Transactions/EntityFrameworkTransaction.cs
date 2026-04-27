using Database.Abstracts;

using Microsoft.EntityFrameworkCore.Storage;

namespace Database.EntityFramework.Transactions;

public class EntityFrameworkTransaction : IDatabaseTransaction
{
    private readonly IDbContextTransaction _transaction;
    
    public EntityFrameworkTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }
    
    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
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
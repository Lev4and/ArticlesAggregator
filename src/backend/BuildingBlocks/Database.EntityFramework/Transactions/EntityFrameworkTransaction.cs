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
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }
}
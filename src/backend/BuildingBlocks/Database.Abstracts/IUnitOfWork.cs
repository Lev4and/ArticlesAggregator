namespace Database.Abstracts;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken ct = default);
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
namespace Database.Abstracts;

public interface IUnitOfWork
{
    Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken ct = default);
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
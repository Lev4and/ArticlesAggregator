namespace Database.Abstracts;

public interface IDatabaseTransaction : IAsyncDisposable, IDisposable
{
    Task CommitAsync(CancellationToken ct = default);
    
    Task RollbackAsync(CancellationToken ct = default);
}
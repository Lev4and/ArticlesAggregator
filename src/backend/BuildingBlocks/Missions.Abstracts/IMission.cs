namespace Missions.Abstracts;

public interface IMission : IAsyncDisposable, IDisposable
{
    Task RunAsync(CancellationToken ct = default);
}
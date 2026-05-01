namespace Missions.Abstracts;

public interface IMission
{
    Task RunAsync(CancellationToken ct = default);
}
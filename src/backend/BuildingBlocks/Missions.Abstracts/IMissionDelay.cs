namespace Missions.Abstracts;

public interface IMissionDelay
{
    Task StartDelayAsync(CancellationToken ct = default);
    
    Task IntervalDelayAsync(CancellationToken ct = default);
}
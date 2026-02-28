namespace Missions.Abstracts;

public interface IMissionDelay
{
    Task StartDelayAsync(CancellationToken cancellationToken = default);
    
    Task IntervalDelayAsync(CancellationToken cancellationToken = default);
}
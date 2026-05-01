using Missions.Abstracts;

namespace Missions.Hosting;

public class MissionDelay : IMissionDelay
{
    private readonly TimeSpan _startDelay;
    private readonly TimeSpan _intervalDelay;
    
    public MissionDelay(TimeSpan startDelay, TimeSpan intervalDelay)
    {
        _startDelay    = startDelay;
        _intervalDelay = intervalDelay;
    }
    
    public async Task StartDelayAsync(CancellationToken ct = default)
    {
        await Task.Delay(_startDelay, ct);
    }

    public async Task IntervalDelayAsync(CancellationToken ct = default)
    {
        await Task.Delay(_intervalDelay, ct);
    }
}
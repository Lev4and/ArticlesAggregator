using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Missions.Abstracts;

namespace Missions.Hosting;

public abstract class MissionWorker<TMission> : BackgroundService
    where TMission : IMission
{
    private readonly IMissionDelay _missionDelay;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public MissionWorker(
        IMissionDelay missionDelay, 
        IServiceScopeFactory serviceScopeFactory)
    {
        _missionDelay = missionDelay;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _missionDelay.StartDelayAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                
                var mission = (IMission)scope.ServiceProvider.GetRequiredService(typeof(TMission));
                
                await RunMissionAsync(mission, stoppingToken);
            }
            catch (Exception exception)
            {
                
            }
            
            await _missionDelay.IntervalDelayAsync(stoppingToken);
        }
    }

    protected virtual async Task RunMissionAsync(IMission mission, CancellationToken stoppingToken = default)
    {
        await mission.RunAsync(stoppingToken);
    }
}
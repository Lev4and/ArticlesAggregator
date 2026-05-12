using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Missions.Abstracts;

namespace Missions.Hosting;

public class MissionWorker<TMission> : BackgroundService
    where TMission : IMission
{
    private readonly IMissionDelay        _missionDelay;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    protected readonly ILogger<MissionWorker<TMission>> Logger;

    public MissionWorker(
        IMissionDelay missionDelay, 
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MissionWorker<TMission>> logger)
    {
        _missionDelay        = missionDelay;
        _serviceScopeFactory = serviceScopeFactory;
        
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Mission worker started Type: {MissionType}", typeof(TMission).Name);
        
        await _missionDelay.StartDelayAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope   = _serviceScopeFactory.CreateAsyncScope();
                await using var mission = (IMission?)scope.ServiceProvider.GetService(typeof(TMission));
                if (mission is null)
                {
                    Logger.LogError("Mission not found");
                    
                    break;
                }
                
                await RunMissionAsync(mission, stoppingToken);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Mission execution failed");
            }
            
            await _missionDelay.IntervalDelayAsync(stoppingToken);
        }
        
        Logger.LogInformation("Mission worker finished Type: {MissionType}", typeof(TMission).Name);
    }

    protected virtual async Task RunMissionAsync(IMission mission, CancellationToken ct = default)
    {
        await mission.RunAsync(ct);
    }
}
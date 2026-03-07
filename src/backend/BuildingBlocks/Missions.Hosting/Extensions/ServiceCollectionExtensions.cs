using Microsoft.Extensions.DependencyInjection;
using Missions.Abstracts;

namespace Missions.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMission<TMission>(IMissionDelay missionDelay)
            where TMission : class, IMission
        {
            return services;
        }
    }
}
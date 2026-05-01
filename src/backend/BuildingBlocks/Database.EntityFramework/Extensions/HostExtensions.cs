using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Database.EntityFramework.Extensions;

public static class HostExtensions
{
    public static void MigrateDatabase<TDbContext>(this IHost host)
        where TDbContext : BaseDbContext
    {
        using var serviceScope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        
        serviceScope.ServiceProvider.GetRequiredService<TDbContext>().Database.Migrate();
    }
}
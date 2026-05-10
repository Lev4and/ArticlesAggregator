using Database.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace ProxyServerSearcher.Persistence;

public class AppDbContext : BaseDbContext
{
    public AppDbContext(
        ITracer<BaseDbContext> tracer, 
        ILogger<BaseDbContext> logger) : 
        base(tracer, logger)
    {
        
    }

    public AppDbContext(
        ITracer<BaseDbContext> tracer, 
        ILogger<BaseDbContext> logger,
        DbContextOptions options) : 
        base(tracer, logger, options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
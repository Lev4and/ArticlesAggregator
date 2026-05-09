using System.Reflection;

using Database.Abstracts;
using Database.EntityFramework.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace Database.EntityFramework;

public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    protected readonly ITracer<BaseDbContext> Tracer;
    protected readonly ILogger<BaseDbContext> Logger;
    
    public BaseDbContext(
        ITracer<BaseDbContext> tracer, 
        ILogger<BaseDbContext> logger)
    {
        Tracer = tracer;
        Logger = logger;
    }

    public BaseDbContext(
        ITracer<BaseDbContext> tracer, 
        ILogger<BaseDbContext> logger, 
        DbContextOptions options) : 
        base(options)
    {
        Tracer = tracer;
        Logger = logger;
    }
    
    public async Task<IDatabaseTransaction> BeginTransactionAsync(
        CancellationToken ct = default)
    {
        using var operation = Tracer.StartOperation("Begin db transaction");
        
        Logger.LogInformation("Begin db transaction");
        
        var transaction = await Database.BeginTransactionAsync(ct);
        
        return new EntityFrameworkTransaction(Tracer, Logger, transaction);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using var operation = Tracer.StartOperation("Save changes in db");
        
        Logger.LogInformation("Save changes in db");
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
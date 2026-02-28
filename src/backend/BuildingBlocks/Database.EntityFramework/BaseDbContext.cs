using System.Reflection;

using Database.Abstracts;
using Database.EntityFramework.Transactions;

using Microsoft.EntityFrameworkCore;

namespace Database.EntityFramework;

public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    public BaseDbContext()
    {
        
    }

    public BaseDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public async Task<IDatabaseTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        
        return new EntityFrameworkTransaction(transaction);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
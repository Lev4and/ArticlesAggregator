using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoredTasks.Database.Abstracts;

namespace StoredTasks.Database.EntityFramework.EntityConfigurations;

public abstract class StoredTaskEntityConfiguration<TStoredTask> : IEntityTypeConfiguration<TStoredTask>
    where TStoredTask : StoredTask
{
    protected abstract string TableName { get; }
    
    public virtual void Configure(EntityTypeBuilder<TStoredTask> builder)
    {
        builder.ToTable(TableName);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.State).HasConversion<string>();
        builder.HasIndex(e => e.State);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.UpdatedAt);
        builder.HasIndex(e => e.WorkerId);
        builder.HasIndex(e => e.AttemptDeadline);
        builder.HasIndex(e => e.Deadline);
        builder.HasIndex(e => e.AttemptsRemaining);
        builder.HasIndex(e => e.Attempts);
        builder.HasIndex(e => e.State);
        builder.HasIndex(e => new { e.State, e.AttemptDeadline, e.AttemptsRemaining, e.Deadline });
    }
}
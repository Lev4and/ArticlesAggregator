using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoredTasks.Database.Abstracts;

namespace StoredTasks.Database.EntityFramework.EntityConfigurations;

public abstract class StoredTaskEntityConfiguration<TStoredTask> : IEntityTypeConfiguration<TStoredTask>
    where TStoredTask : StoredTask
{
    protected abstract string TableName { get; }
    
    public void Configure(EntityTypeBuilder<TStoredTask> builder)
    {
        throw new NotImplementedException();
    }
}
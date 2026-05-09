using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProxyServerSearcher.Domain.Entities;
using ProxyServerSearcher.Persistence.Constants;
using StoredTasks.Database.EntityFramework.EntityConfigurations;

namespace ProxyServerSearcher.Persistence.EntityConfigurations;

public class ProxyServerSearchStoredTaskEntityConfiguration : StoredTaskEntityConfiguration<ProxyServerSearchStoredTask>
{
    protected override string TableName => TableNameConstants.ProxyServerSearchStoredTasks;

    public override void Configure(EntityTypeBuilder<ProxyServerSearchStoredTask> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => e.SourceName);
    }
}
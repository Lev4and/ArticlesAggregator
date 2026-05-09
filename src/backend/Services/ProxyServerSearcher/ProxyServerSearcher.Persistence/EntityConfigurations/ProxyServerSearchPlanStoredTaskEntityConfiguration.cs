using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProxyServerSearcher.Domain.Entities;
using ProxyServerSearcher.Persistence.Constants;
using StoredTasks.Database.EntityFramework.EntityConfigurations;

namespace ProxyServerSearcher.Persistence.EntityConfigurations;

public class ProxyServerSearchPlanStoredTaskEntityConfiguration : 
    StoredTaskEntityConfiguration<ProxyServerSearchPlanStoredTask>
{
    protected override string TableName => TableNameConstants.ProxyServerSearchPlanStoredTasks;

    public override void Configure(EntityTypeBuilder<ProxyServerSearchPlanStoredTask> builder)
    {
        base.Configure(builder);
        
        builder.HasIndex(e => e.PlannedAt).IsUnique();
    }
}
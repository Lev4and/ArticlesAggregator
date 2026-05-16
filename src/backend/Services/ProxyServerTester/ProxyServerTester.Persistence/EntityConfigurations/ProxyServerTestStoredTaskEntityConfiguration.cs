using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProxyServerTester.Domain.Entities;
using ProxyServerTester.Persistence.Constants;
using StoredTasks.Database.EntityFramework.EntityConfigurations;

namespace ProxyServerTester.Persistence.EntityConfigurations;

public class ProxyServerTestStoredTaskEntityConfiguration : StoredTaskEntityConfiguration<ProxyServerTestStoredTask>
{
    protected override string TableName => TableNameConstants.ProxyServerTestStoredTasks;

    public override void Configure(EntityTypeBuilder<ProxyServerTestStoredTask> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => e.RequestId);
    }
}
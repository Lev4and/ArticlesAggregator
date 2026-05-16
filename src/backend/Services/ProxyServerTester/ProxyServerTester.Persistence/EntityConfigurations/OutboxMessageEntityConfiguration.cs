using Messaging.Outbox.Abstracts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProxyServerTester.Persistence.Constants;
using StoredTasks.Database.EntityFramework.EntityConfigurations;

namespace ProxyServerTester.Persistence.EntityConfigurations;

public class OutboxMessageEntityConfiguration : StoredTaskEntityConfiguration<OutboxMessage>
{
    protected override string TableName => TableNameConstants.OutboxMessages;
    
    public override void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        base.Configure(builder);
        
        builder.HasIndex(e => e.Type);
    }
}
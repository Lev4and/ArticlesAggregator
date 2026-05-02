using DomainEvents.Database.Abstracts;
using DomainEvents.Database.EntityFramework.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoredTasks.Database.EntityFramework.EntityConfigurations;

namespace DomainEvents.Database.EntityFramework.EntityConfigurations;

public class DomainEventEntityConfiguration : StoredTaskEntityConfiguration<DomainEvent>
{
    protected override string TableName => TableNameConstants.DomainEvents;

    public override void Configure(EntityTypeBuilder<DomainEvent> builder)
    {
        base.Configure(builder);

        builder.HasIndex(t => t.Type);
    }
}
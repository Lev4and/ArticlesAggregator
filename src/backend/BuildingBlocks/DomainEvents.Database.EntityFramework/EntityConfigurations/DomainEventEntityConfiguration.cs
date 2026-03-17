using DomainEvents.Database.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainEvents.Database.EntityFramework.EntityConfigurations;

public class DomainEventEntityConfiguration : IEntityTypeConfiguration<DomainEvent>
{
    public void Configure(EntityTypeBuilder<DomainEvent> builder)
    {
        throw new NotImplementedException();
    }
}
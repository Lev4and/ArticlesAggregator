using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProxyServerTester.Domain.Entities;
using ProxyServerTester.Persistence.Constants;

namespace ProxyServerTester.Persistence.EntityConfigurations;

public class ProxyServerTestRequestEntityConfiguration : IEntityTypeConfiguration<ProxyServerTestRequest>
{
    public void Configure(EntityTypeBuilder<ProxyServerTestRequest> builder)
    {
        builder.ToTable(TableNameConstants.ProxyServerTestRequests);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<string>();
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.RequestTime);
        builder.HasIndex(e => e.ResponseTime);
        builder.Property(e => e.EntityState).HasConversion<string>();
        builder.HasIndex(e => e.EntityState);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.UpdatedAt);
        builder.HasIndex(e => e.IsDeleted);
        builder.HasIndex(e => e.DeletedAt);
        builder.HasQueryFilter(e => !e.IsDeleted);
        builder
            .HasOne(e => e.Task)
                .WithOne(e => e.Request)
                    .HasForeignKey<ProxyServerTestStoredTask>(e => e.RequestId)
                        .OnDelete(DeleteBehavior.Cascade);
    }
}
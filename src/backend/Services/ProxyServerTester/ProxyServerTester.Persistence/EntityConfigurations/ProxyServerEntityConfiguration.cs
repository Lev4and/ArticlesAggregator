using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProxyServerTester.Domain.Entities;
using ProxyServerTester.Persistence.Constants;

namespace ProxyServerTester.Persistence.EntityConfigurations;

public class ProxyServerEntityConfiguration : IEntityTypeConfiguration<ProxyServer>
{
    public void Configure(EntityTypeBuilder<ProxyServer> builder)
    {
        builder.ToTable(TableNameConstants.ProxyServers);
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.NormalizedName);
        builder.Property(e => e.Protocol).HasConversion<string>();
        builder.HasIndex(e => e.Protocol);
        builder.HasIndex(e => e.HostnameOrAddress);
        builder.HasIndex(e => e.Port);
        builder.OwnsOne(e => e.Credentials, ownedNavigationBuilder =>
        {
            ownedNavigationBuilder.HasIndex(e => e.Username);
            ownedNavigationBuilder.HasIndex(e => e.Password);
        });
        builder.Property(e => e.EntityState).HasConversion<string>();
        builder.HasIndex(e => e.EntityState);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.UpdatedAt);
        builder.HasIndex(e => e.IsDeleted);
        builder.HasIndex(e => e.DeletedAt);
        builder.HasQueryFilter(e => !e.IsDeleted);
        builder
            .HasMany(e => e.TestRequests)
                .WithOne(e => e.Server)
                    .HasPrincipalKey(e => e.Id)
                        .OnDelete(DeleteBehavior.Cascade);
    }
}
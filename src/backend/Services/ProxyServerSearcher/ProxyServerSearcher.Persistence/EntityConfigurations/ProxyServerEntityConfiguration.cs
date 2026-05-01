using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProxyServerSearcher.Domain.Entities;
using ProxyServerSearcher.Persistence.Constants;

namespace ProxyServerSearcher.Persistence.EntityConfigurations;

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
        builder.OwnsOne(e => e.Credentials);
        builder.HasIndex(e => e.EntityState);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.UpdatedAt);
        builder.HasIndex(e => e.IsDeleted);
        builder.HasIndex(e => e.DeletedAt);
        builder.HasQueryFilter(entity => !entity.IsDeleted);
    }
}
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
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Protocol).HasConversion<string>();
        builder.HasIndex(entity => entity.Protocol);
        builder.HasIndex(entity => new { entity.Host, entity.Port }).IsUnique();
        builder.OwnsOne(entity => entity.Credentials);
        builder.Property(entity => entity.EntityState).HasConversion<string>();
        builder.HasIndex(entity => entity.EntityState);
        builder.HasIndex(entity => entity.CreatedAt);
        builder.HasIndex(entity => entity.UpdatedAt);
        builder.HasIndex(entity => entity.IsDeleted);
        builder.Property(entity => entity.DeletedAt);
    }
}
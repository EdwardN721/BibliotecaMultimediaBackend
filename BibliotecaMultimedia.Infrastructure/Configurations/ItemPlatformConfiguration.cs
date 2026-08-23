using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class ItemPlatformConfiguration : IEntityTypeConfiguration<ItemPlatform>
{
    public void Configure(EntityTypeBuilder<ItemPlatform> builder)
    {
        builder.ToTable("item_platforms");

        builder.Property(ip => ip.Id).HasDefaultValueSql("gen_random_uuid()");

        // Constraint de unicidad: Un mismo Item no puede repetir la misma Plataforma
        builder.HasIndex(ip => new { ip.ItemId, ip.PlatformId })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}

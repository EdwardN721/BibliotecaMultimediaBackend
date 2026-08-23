using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class ItemFormatConfiguration : IEntityTypeConfiguration<ItemFormat>
{
    public void Configure(EntityTypeBuilder<ItemFormat> builder)
    {
        builder.ToTable("item_formats");

        builder.Property(ifm => ifm.Id).HasDefaultValueSql("gen_random_uuid()");

        // Constraint de unicidad: Un mismo Item no puede repetir el mismo Formato
        builder.HasIndex(ifm => new { ifm.ItemId, ifm.FormatId })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}

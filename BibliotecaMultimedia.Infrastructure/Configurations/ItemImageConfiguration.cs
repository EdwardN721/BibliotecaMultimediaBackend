using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class ItemImageConfiguration : IEntityTypeConfiguration<ItemImage>
{
    public void Configure(EntityTypeBuilder<ItemImage> builder)
    {
        builder.ToTable("item_images");

        builder.Property(ii => ii.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(ii => ii.ImageUrl)
            .IsRequired()
            .HasMaxLength(1000); // Las URLs pueden ser largas

        builder.Property(ii => ii.IsPrimary)
            .HasDefaultValue(false);

        // Índice Único Parcial: Solo permite un IsPrimary = true por cada ItemId
        builder.HasIndex(ii => ii.ItemId)
            .IsUnique()
            .HasFilter("\"IsPrimary\" = true");
    }
}
using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items");

        builder.Property(i => i.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(255);

        // Mapeo crucial: Decirle a Npgsql que use jsonb para el JsonDocument
        builder.Property(i => i.Metadata)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb");
        
        builder.HasIndex(i => i.UserId);
        builder.HasIndex(i => i.MediaTypeId);
        
        builder.HasOne(i => i.User).WithMany(u => u.Items).HasForeignKey(i => i.UserId);
        builder.HasOne(i => i.MediaType).WithMany().HasForeignKey(i => i.MediaTypeId);
        builder.HasOne(i => i.Format).WithMany().HasForeignKey(i => i.FormatId);
        builder.HasOne(i => i.Platform).WithMany().HasForeignKey(i => i.PlatformId);
    }
}
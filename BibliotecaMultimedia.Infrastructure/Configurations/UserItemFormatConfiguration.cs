using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class UserItemFormatConfiguration : IEntityTypeConfiguration<UserItemFormat>
{
    public void Configure(EntityTypeBuilder<UserItemFormat> builder)
    {
        builder.ToTable("user_item_formats");

        builder.Property(uif => uif.Id).HasDefaultValueSql("gen_random_uuid()");

        // Constraint de unicidad: un mismo UserItem no puede repetir el mismo formato
        builder.HasIndex(uif => new { uif.UserItemId, uif.FormatId })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}

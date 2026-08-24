using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class UserItemPlatformConfiguration : IEntityTypeConfiguration<UserItemPlatform>
{
    public void Configure(EntityTypeBuilder<UserItemPlatform> builder)
    {
        builder.ToTable("user_item_platforms");

        builder.Property(uip => uip.Id).HasDefaultValueSql("gen_random_uuid()");

        // Constraint de unicidad: un mismo UserItem no puede repetir la misma plataforma
        builder.HasIndex(uip => new { uip.UserItemId, uip.PlatformId })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}

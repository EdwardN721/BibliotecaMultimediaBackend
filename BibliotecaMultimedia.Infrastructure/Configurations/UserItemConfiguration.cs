using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class UserItemConfiguration : IEntityTypeConfiguration<UserItem>
{
    public void Configure(EntityTypeBuilder<UserItem> builder)
    {
        builder.ToTable("user_items");

        builder.Property(ui => ui.Id).HasDefaultValueSql("gen_random_uuid()");

        // Índice parcial: las filas con soft-delete no bloquean re-agregar el mismo item
        builder.HasIndex(u => new { u.UserId, u.ItemId })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.Property(u => u.Status)
            .HasConversion<string>();

        builder.HasOne(ui => ui.User)
            .WithMany(u => u.UserItems)
            .HasForeignKey(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ui => ui.Item)
            .WithMany(i => i.UserItems)
            .HasForeignKey(ui => ui.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
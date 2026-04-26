using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class ItemCreatorConfiguration : IEntityTypeConfiguration<ItemCreator>
{
    public void Configure(EntityTypeBuilder<ItemCreator> builder)
    {
        builder.ToTable("item_creators");

        builder.Property(ic => ic.Id).HasDefaultValueSql("gen_random_uuid()");

        // Constraint Único: Evita duplicidad lógica (Mismo Item, Creador y Rol)
        builder.HasIndex(ic => new { ic.ItemId, ic.CreatorId, ic.RoleId })
            .IsUnique();
    }
}
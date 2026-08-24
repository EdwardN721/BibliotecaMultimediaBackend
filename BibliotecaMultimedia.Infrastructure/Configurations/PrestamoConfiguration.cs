using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class PrestamoConfiguration : IEntityTypeConfiguration<Prestamo>
{
    public void Configure(EntityTypeBuilder<Prestamo> builder)
    {
        builder.ToTable("prestamos");

        builder.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");

        // Consultas típicas: préstamos de un título, activos primero
        builder.HasIndex(p => p.UserItemId);

        builder.Property(p => p.NombrePersona).HasMaxLength(120);
        builder.Property(p => p.Notas).HasMaxLength(500);

        builder.HasOne(p => p.UserItem)
            .WithMany(ui => ui.Prestamos)
            .HasForeignKey(p => p.UserItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

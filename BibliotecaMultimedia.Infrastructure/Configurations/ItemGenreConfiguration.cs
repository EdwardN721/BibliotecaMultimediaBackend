using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliotecaMultimedia.Infrastructure.Configurations;

public class ItemGenreConfiguration : IEntityTypeConfiguration<ItemGenre>
{
    public void Configure(EntityTypeBuilder<ItemGenre> builder)
    {
        builder.ToTable("item_genres");

        builder.Property(ig => ig.Id).HasDefaultValueSql("gen_random_uuid()");

        // Constraint de unicidad: Un mismo Item no puede repetir el mismo Genero
        builder.HasIndex(ig => new { ig.ItemId, ig.GenreId }).IsUnique();
    }
}
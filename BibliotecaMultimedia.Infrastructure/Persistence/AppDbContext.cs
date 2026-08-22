using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BibliotecaMultimedia.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BibliotecaMultimedia.Infrastructure.Persistence;

// Forzar que los IDs de Identity sean Guid
public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Creator> Creators => Set<Creator>();
    public DbSet<MediaType> MediaTypes => Set<MediaType>();
    public DbSet<Format> Formats => Set<Format>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Role> CreatorRoles => Set<Role>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<UserItem> UserItems => Set<UserItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 
        
        // Esto busca automáticamente todas las clases que implementen IEntityTypeConfiguration en el proyecto
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Soft-delete: excluir de las consultas las entidades del dominio eliminadas
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parametro = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var propiedad = System.Linq.Expressions.Expression.Property(parametro, nameof(BaseEntity.DeletedAt));
                var nulo = System.Linq.Expressions.Expression.Constant(null, typeof(DateTimeOffset?));
                var cuerpo = System.Linq.Expressions.Expression.Equal(propiedad, nulo);
                var filtro = System.Linq.Expressions.Expression.Lambda(cuerpo, parametro);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filtro);
            }
        }
    }
}
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
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 
        
        // Esto busca automáticamente todas las clases que implementen IEntityTypeConfiguration en el proyecto
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
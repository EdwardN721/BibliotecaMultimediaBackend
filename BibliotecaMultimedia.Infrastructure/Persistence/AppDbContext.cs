using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Infrastructure.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMultimedia.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext
{
    private readonly UserSessionInterceptor _sessionInterceptor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        UserSessionInterceptor sessionInterceptor) : base(options)
    {
        _sessionInterceptor = sessionInterceptor;
    }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Creator> Creators => Set<Creator>();
    public DbSet<MediaType> MediaTypes => Set<MediaType>();
    public DbSet<Format> Formats => Set<Format>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Agregamos nuestro interceptor a la cadena de ejecución
        optionsBuilder.AddInterceptors(_sessionInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Esto busca automáticamente todas las clases que implementen IEntityTypeConfiguration en este proyecto
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
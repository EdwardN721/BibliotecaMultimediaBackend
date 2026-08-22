using BibliotecaMultimedia.Domain.Constants;
using BibliotecaMultimedia.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BibliotecaMultimedia.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedCatalogoAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseSeeder");

        string[] nombresRoles = { RoleConstants.Director, RoleConstants.Author, RoleConstants.Composer, RoleConstants.Developer };
        foreach (string nombreRol in nombresRoles)
        {
            bool existe = await dbContext.CreatorRoles.AnyAsync(r => r.Name == nombreRol, cancellationToken);
            if (!existe)
            {
                dbContext.CreatorRoles.Add(new Role { Name = nombreRol });
                logger.LogInformation("Rol de catálogo creado: {Nombre}", nombreRol);
            }
        }

        string[] nombresMediaTypes = { MediaTypeConstants.Book, MediaTypeConstants.Movie, MediaTypeConstants.VideoGame, MediaTypeConstants.Music };
        foreach (string nombreMediaType in nombresMediaTypes)
        {
            bool existe = await dbContext.MediaTypes.AnyAsync(m => m.Name == nombreMediaType, cancellationToken);
            if (!existe)
            {
                dbContext.MediaTypes.Add(new MediaType { Name = nombreMediaType });
                logger.LogInformation("Tipo de medio creado: {Nombre}", nombreMediaType);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
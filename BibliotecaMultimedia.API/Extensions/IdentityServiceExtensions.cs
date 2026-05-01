using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaMultimedia.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        // Configuramos Identity : User y rol con Guid
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                // Configurar las políticas de contraseñas 
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            
                // Requerimos que el email sea único en la base de datos
                options.User.RequireUniqueEmail = true; 
            })
            .AddEntityFrameworkStores<AppDbContext>() 
            .AddDefaultTokenProviders(); // generar tokens de "Olvidé mi contraseña"
        return services;
    }
}
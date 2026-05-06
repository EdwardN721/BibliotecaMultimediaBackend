using System.Text;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BibliotecaMultimedia.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, 
        IConfiguration configuration)
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
        
        // Leer Token
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // Verifica que no esté expirado
                    ValidateIssuerSigningKey = true, // Valida la firma del token

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ??
                                               throw new InvalidOperationException("Falta Jwt:Key en configuración")))
                };
            });
        return services;
    }
}
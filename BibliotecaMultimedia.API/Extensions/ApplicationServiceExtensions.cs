using Asp.Versioning;
using BibliotecaMultimedia.API.Handlers;
using BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Service;
using FluentValidation;
using Microsoft.OpenApi;

namespace BibliotecaMultimedia.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BibliotecaMultimedia.API",
                Version = "v1",
            });
        });
        
        return services;
    }

    public static IServiceCollection AddValidations(this IServiceCollection services)
    {
        // Busca automáticamente todas las clases que hereden de AbstractValidator 
        var applicationAssembly = typeof(PeticionCrearUsuarioDto).Assembly;
        services.AddValidatorsFromAssembly(applicationAssembly);
        
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }

    public static IServiceCollection AddGlobalException(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        return services;
    }

    public static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); // Versión por defecto: 1.0
                options.AssumeDefaultVersionWhenUnspecified = true; // Si no mandan versión, asume la 1.0
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(), // Lee la versión de la URL (ej: api/v1/...)
                    new HeaderApiVersionReader("X-Api-Version") // Lee de los headers
                );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        return services;
    }
}
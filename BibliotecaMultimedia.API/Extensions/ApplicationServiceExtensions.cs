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
}
using Asp.Versioning;
using FluentValidation;
using Microsoft.OpenApi;
using BibliotecaMultimedia.API.Handlers;
using BibliotecaMultimedia.Application.Service;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Validators;

namespace BibliotecaMultimedia.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        // Usamos la API nativa de .NET para generar el documento
        services.AddOpenApi(options =>
        {
            // Así se inyecta la seguridad en la v2 de Microsoft.OpenApi
            options.AddDocumentTransformer((document, _, _) =>
            {
                // 1. Definimos el esquema
                var scheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Ingresa tu token JWT aquí (sin la palabra Bearer)."
                };

                // 2. ERROR 1 RESUELTO: Ahora exige usar el diccionario con IOpenApiSecurityScheme
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes["Bearer"] = scheme;

                // 3. ERROR 2 RESUELTO: El diccionario del requerimiento ahora pide explícitamente la clase de referencia
                var requirement = new OpenApiSecurityRequirement
                {
                    // Nota: Si el compilador te marca un pequeño error aquí, cámbialo a: new OpenApiSecuritySchemeReference("Bearer")
                    [new OpenApiSecuritySchemeReference("Bearer")] = new List<string>() 
                };

                // 4. ERROR 3 RESUELTO: La propiedad 'SecurityRequirements' fue renombrada a 'Security'
                document.Security ??= new List<OpenApiSecurityRequirement>();
                document.Security.Add(requirement);

                return Task.CompletedTask;
            });
        });

        return services;
    }
    
    public static IServiceCollection AddValidations(this IServiceCollection services)
    {
        // Busca automáticamente todas las clases que hereden de AbstractValidator 
        var applicationAssembly = typeof(UsuarioValidator).Assembly;
        services.AddValidatorsFromAssembly(applicationAssembly);
        
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGeneroService, GeneroService>();
        
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
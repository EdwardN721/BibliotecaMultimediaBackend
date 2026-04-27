using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Infrastructure.Repositoy;
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

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    } 
}
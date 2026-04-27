using Microsoft.OpenApi;

namespace BibliotecaMultimedia.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        services.AddControllers();
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
}
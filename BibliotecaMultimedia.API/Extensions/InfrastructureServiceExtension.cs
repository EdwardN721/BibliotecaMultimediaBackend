using BibliotecaMultimedia.Infrastructure.Interceptors;
using BibliotecaMultimedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMultimedia.API.Extensions;

public static class InfrastructureServiceExtension
{
    public static IServiceCollection AddInterceptors(this IServiceCollection services)
    {
        services.AddScoped<UserSessionInterceptor>();
        
        return services;
    }

    public static IServiceCollection AddDbPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var interceptor = serviceProvider.GetService<UserSessionInterceptor>()!;

            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(interceptor);
        });
        
        return services;
    }
}
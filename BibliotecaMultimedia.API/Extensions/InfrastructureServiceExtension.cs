using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Infrastructure.Interceptors;
using BibliotecaMultimedia.Infrastructure.Persistence;
using BibliotecaMultimedia.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMultimedia.API.Extensions;

public static class InfrastructureServiceExtension
{
    public static IServiceCollection AddInterceptors(this IServiceCollection services)
    {
        services.AddScoped<UserSessionInterceptor>();
        services.AddScoped<AuditInterceptor>();
        
        return services;
    }

    public static IServiceCollection AddDbPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var interceptorUserSession = serviceProvider.GetService<UserSessionInterceptor>()!;
            var interceptorAudit = serviceProvider.GetService<AuditInterceptor>()!;

            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(interceptorUserSession, interceptorAudit);
        });
        
        return services;
    }
    
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    } 
}
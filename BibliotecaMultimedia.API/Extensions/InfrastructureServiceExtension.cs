using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Infrastructure.Interceptors;
using BibliotecaMultimedia.Infrastructure.Persistence;
using BibliotecaMultimedia.Infrastructure.Repository;
using BibliotecaMultimedia.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMultimedia.API.Extensions;

public static class InfrastructureServiceExtension
{
    public static IServiceCollection AddInterceptors(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
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

    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Registrar Blob Storage como Singleton (es thread-safe y recomendado por Microsoft)
        services.AddSingleton<IBlobStorageService>(_ => 
        {
            string blobConnectionString = configuration.GetConnectionString("AzureBlobStorage") 
                                          ?? throw new BusinessRuleException("La cadena de conexión de AzureBlobStorage no está configurada.");
            string blobContainerString = configuration["ConnectionStrings.AzureBlobStorageContainer"] ?? throw new BusinessRuleException("El Contenedor de Azure BlobStorage no está configurado.");
            
            return new BlobStorageService(blobConnectionString, blobContainerString);
        });

        // 2. Registrar Service Bus como Singleton
        services.AddSingleton<IServiceBus>(_ => 
        {
            string busConnectionString = configuration.GetConnectionString("AzureServiceBus")
                                         ?? throw new BusinessRuleException("La cadena de conexión de AzureServiceBus no está configurada.");
            
            // Extraemos el nombre de la cola
            string queueName = configuration["Azure:ServiceBus:QueueName"]
                               ?? throw new BusinessRuleException("El QueueName de Azure Service Bus no está configurado.");

            return new ServiceBus(busConnectionString, queueName);
        });

        return services;
    }
}
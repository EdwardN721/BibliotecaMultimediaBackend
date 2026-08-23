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
        // Validación temprana (fail-fast): si falta alguna clave de Azure la API
        // no arranca. Evita 409 dispersos en runtime cuando un endpoint resuelve
        // por primera vez el singleton con la factory perezosa.
        string blobConnectionString = configuration.GetConnectionString("AzureBlobStorage")
                                      ?? throw new BusinessRuleException("La cadena de conexión de AzureBlobStorage no está configurada.");

        // GetConnectionString equivale a configuration["ConnectionStrings:<nombre>"]
        string blobContainerString = configuration.GetConnectionString("AzureBlobStorageContainer")
                                     ?? throw new BusinessRuleException("El Contenedor de Azure BlobStorage no está configurado.");

        string busConnectionString = configuration.GetConnectionString("AzureServiceBus")
                                     ?? throw new BusinessRuleException("La cadena de conexión de AzureServiceBus no está configurada.");

        string queueName = configuration["Azure:ServiceBus:QueueName"]
                           ?? throw new BusinessRuleException("El QueueName de Azure Service Bus no está configurado.");

        // Blob Storage como Singleton: es thread-safe y su reutilización es la
        // práctica recomendada por Microsoft.
        services.AddSingleton<IBlobStorageService>(_ =>
            new BlobStorageService(blobConnectionString, blobContainerString));

        services.AddSingleton<IServiceBus>(_ =>
            new ServiceBus(busConnectionString, queueName));

        return services;
    }
}
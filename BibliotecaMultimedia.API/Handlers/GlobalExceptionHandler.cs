using BibliotecaMultimedia.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BibliotecaMultimedia.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // 1. Creamos logs: los errores esperados (validación, no encontrado, conflicto)
        //    son Warning; lo inesperado sí es Error para alertar en operación
        bool esEsperado = exception is ValidationAppException
            or IdentityUserException
            or UnauthorizedAppException
            or NotFoundException
            or BusinessRuleException
            or OperationCanceledException;

        if (esEsperado)
        {
            _logger.LogWarning("Solicitud rechazada ({Tipo}): {Message}", exception.GetType().Name, exception.Message);
        }
        else
        {
            _logger.LogError(exception, "Ocurrió un error: {Message}", exception.Message);
        }

        // 2. Dar formato estandarizado
        ProblemDetails problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
        };

        // 3. Filtramos el tipo de excepción para dar la respuesta HTTP correcta
        if (exception is ValidationAppException validationAppException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Error de validación";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = validationAppException.Message;
            problemDetails.Extensions["errors"] = validationAppException.Errors; // Inyectamos el diccionario de errores
        }
        else if (exception is IdentityUserException identityException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Error de Registro";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = identityException.Message;
            problemDetails.Extensions["errors"] = identityException.Errors; // Inyectamos el diccionario de errores de Identity
        }
        else if (exception is UnauthorizedAppException unauthorizedException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            problemDetails.Title = "No Autorizado";
            problemDetails.Status = StatusCodes.Status401Unauthorized;
            problemDetails.Detail = unauthorizedException.Message;
        }
        else if (exception is NotFoundException notFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            problemDetails.Title = "Recurso no encontrado";
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Detail = notFoundException.Message;
        }
        else if (exception is BusinessRuleException businessRuleException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            problemDetails.Title = "Conflicto de regla de negocio";
            problemDetails.Status = StatusCodes.Status409Conflict;
            problemDetails.Detail = businessRuleException.Message;
        }
        else if (exception is DbUpdateException dbUpdateException)
        {
            // Violaciones de índices únicos (ej. duplicado en la biblioteca por carrera entre check e insert)
            bool violacionUnicidad = dbUpdateException.InnerException is PostgresException { SqlState: "23505" };
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            problemDetails.Title = "Conflicto de integridad de datos";
            problemDetails.Status = StatusCodes.Status409Conflict;
            problemDetails.Detail = violacionUnicidad
                ? "El registro ya existe o viola una restricción de unicidad."
                : "La operación no puede completarse porque viola una restricción de integridad de datos.";
        }
        else if (exception is OperationCanceledException operationCanceledException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            problemDetails.Title = "La operación fue cancelada";
            problemDetails.Status = httpContext.Response.StatusCode;
            problemDetails.Detail = "El cliente canceló la solicitud antes de que se completara.";
        }
        else
        {
            // Error genérico para atrapar cosas inesperadas (ej. la base de datos se cayó)
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "Error Interno del Servidor";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = "Ha ocurrido un error inesperado. Por favor contacte al soporte.";
        }

        // 4. Escribimos la respuesta en formato JSON
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
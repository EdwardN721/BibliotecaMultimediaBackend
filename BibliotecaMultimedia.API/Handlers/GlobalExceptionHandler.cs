using BibliotecaMultimedia.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
        // 1. Creamos logs para saber donde hubo error
        _logger.LogError(exception, "Ocurrió un error: {Message}", exception.Message);
        
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
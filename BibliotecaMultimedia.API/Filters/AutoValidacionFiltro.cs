using BibliotecaMultimedia.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecaMultimedia.API.Filters;

/// <summary>
/// Ejecuta automáticamente el IValidator&lt;T&gt; registrado en DI para cada argumento
/// complejo de una acción. Si falla, lanza ValidationAppException que el
/// GlobalExceptionHandler convierte en 400 ProblemDetails con errores por campo.
/// Los tipos sin validator registrado (Guid, primitivos, CancellationToken) se ignoran.
/// </summary>
public class AutoValidacionFiltro : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (object? argumento in context.ActionArguments.Values)
        {
            if (argumento is null or CancellationToken) continue;

            Type tipo = argumento.GetType();
            if (tipo.IsPrimitive || tipo == typeof(string) || tipo == typeof(decimal) || tipo == typeof(Guid))
                continue;

            Type tipoValidator = typeof(IValidator<>).MakeGenericType(tipo);
            IValidator? validator = context.HttpContext.RequestServices.GetService(tipoValidator) as IValidator;
            if (validator is null) continue;

            FluentValidation.Results.ValidationResult resultado =
                await validator.ValidateAsync(new ValidationContext<object>(argumento));

            if (!resultado.IsValid)
                throw new ValidationAppException(resultado.Errors);
        }

        await next();
    }
}

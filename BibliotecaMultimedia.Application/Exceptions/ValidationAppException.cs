using FluentValidation.Results;

namespace BibliotecaMultimedia.Application.Exceptions;

/// <summary>
/// Atrapa los errores de validacion
/// </summary>
public class ValidationAppException : Exception
{
    // Un diccionario para agrupar los errores por propiedad (ej. "Email": ["El correo es inválido", "No puede estar vacío"])
    public IDictionary<string, string[]> Errors { get; }

    public ValidationAppException(IEnumerable<ValidationFailure> failures) 
        : base("Se encontraron uno o más errores de validación.")
    {
           Errors = failures
               .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
               .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
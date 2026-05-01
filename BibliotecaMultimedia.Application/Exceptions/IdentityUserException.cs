using Microsoft.AspNetCore.Identity;

namespace BibliotecaMultimedia.Application.Exceptions;

public class IdentityUserException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public IdentityUserException(IEnumerable<IdentityError> failures)
        : base("Se encontraron uno o más errores al crear el usuario.")
    {
        Errors =  failures
            .GroupBy(e => e.Code, e => e.Description)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
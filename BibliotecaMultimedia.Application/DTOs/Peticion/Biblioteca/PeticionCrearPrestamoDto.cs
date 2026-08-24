namespace BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;

/// <summary>
/// Registrar un nuevo préstamo. La fecha de préstamo es opcional:
/// si no se envía, se usa la fecha actual.
/// </summary>
public record PeticionCrearPrestamoDto
{
    public required string NombrePersona { get; init; }
    public string? Notas { get; init; }
    public DateTimeOffset? FechaPrestamo { get; init; }
};

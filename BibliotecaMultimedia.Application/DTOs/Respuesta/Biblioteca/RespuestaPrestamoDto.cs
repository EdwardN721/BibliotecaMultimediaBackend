namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;

public record RespuestaPrestamoDto
{
    public Guid Id { get; init; }
    public Guid UserItemId { get; init; }
    public string NombrePersona { get; init; } = string.Empty;
    public DateTimeOffset FechaPrestamo { get; init; }
    public DateTimeOffset? FechaDevolucion { get; init; }
    public string? Notas { get; init; }
    public bool EstaActivo => FechaDevolucion is null;
};

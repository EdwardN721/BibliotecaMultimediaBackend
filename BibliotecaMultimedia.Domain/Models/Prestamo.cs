namespace BibliotecaMultimedia.Domain.Models;

/// <summary>
/// Registro de un préstamo de un título de la biblioteca del usuario:
/// a quién se prestó, cuándo y si ya fue devuelto (FechaDevolucion null = sigue prestado).
/// </summary>
public class Prestamo : BaseEntity
{
    public required Guid UserItemId { get; set; }
    public UserItem? UserItem { get; set; }

    public required string NombrePersona { get; set; }
    public DateTimeOffset FechaPrestamo { get; set; }
    public DateTimeOffset? FechaDevolucion { get; set; }
    public string? Notas { get; set; }
}

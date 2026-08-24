namespace BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;

/// <summary>
/// Corregir los datos de un préstamo existente (persona/notas).
/// La devolución se registra con el endpoint dedicado PUT prestamos/{id}/devolucion.
/// </summary>
public record PeticionActualizarPrestamoDto
{
    public string? NombrePersona { get; init; }
    public string? Notas { get; init; }
};

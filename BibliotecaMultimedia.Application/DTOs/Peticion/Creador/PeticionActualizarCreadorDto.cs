namespace BibliotecaMultimedia.Application.DTOs.Peticion.Creador;

public record PeticionActualizarCreadorDto
{
    public string Nombre { get; init; } = string.Empty;
    public string? Biografia { get; init; } = string.Empty;
};
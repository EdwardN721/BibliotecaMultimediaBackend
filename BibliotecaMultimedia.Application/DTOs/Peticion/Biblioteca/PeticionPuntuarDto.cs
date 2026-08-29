namespace BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;

/// <summary>
/// Puntuación personal de un elemento de la biblioteca (1 a 5).
/// </summary>
public record PeticionPuntuarDto
{
    public short Rating { get; init; }
};

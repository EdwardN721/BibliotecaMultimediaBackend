namespace BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;

public record PeticionActualizarFormatoDto
{
    public string Nombre { get; init; } = string.Empty;
};
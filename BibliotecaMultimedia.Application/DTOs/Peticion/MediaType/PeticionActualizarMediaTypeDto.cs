namespace BibliotecaMultimedia.Application.DTOs.Peticion.MediaType;

public record PeticionActualizarMediaTypeDto
{
    public string Nombre { get; init; } = string.Empty;
};
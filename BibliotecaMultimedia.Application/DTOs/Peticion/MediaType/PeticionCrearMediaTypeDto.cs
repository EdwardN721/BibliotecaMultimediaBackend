namespace BibliotecaMultimedia.Application.DTOs.Peticion.MediaType;

public record PeticionCrearMediaTypeDto
{
    public string Nombre { get; init; } = string.Empty;
};
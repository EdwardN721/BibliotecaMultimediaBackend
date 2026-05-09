namespace BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;

public record PeticionCrearFormatoDto
{
    public string Nombre { get; init; } = string.Empty;
};
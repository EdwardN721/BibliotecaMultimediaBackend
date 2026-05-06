namespace BibliotecaMultimedia.Application.DTOs.Peticion.Catalogos;

public record PeticionCrearGeneroDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
namespace BibliotecaMultimedia.Application.DTOs.Peticion.Catalogos;

public record PeticionActualizarGeneroDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
}
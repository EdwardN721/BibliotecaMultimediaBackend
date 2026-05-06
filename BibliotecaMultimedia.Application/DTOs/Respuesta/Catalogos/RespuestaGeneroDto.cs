namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Catalogos;

public record RespuestaGeneroDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
namespace BibliotecaMultimedia.Application.DTOs.Respuesta.MediaType;

public record RespuestaMediaTypeDto
{
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
};
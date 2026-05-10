namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Creador;

public record RespuestaCreadorDto
{
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Biografia { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
};
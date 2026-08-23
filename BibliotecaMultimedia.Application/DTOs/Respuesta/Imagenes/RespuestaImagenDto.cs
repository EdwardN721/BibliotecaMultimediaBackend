namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Imagenes;

public record RespuestaImagenDto
{
    public Guid Id { get; init; }
    public Guid ItemId { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public bool IsPrimary { get; init; } = false;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
};

public record RespuestaUploadChunkDto
{
    public bool CargaCompletada { get; init; }
    public string Mensaje { get; init; } = string.Empty;
    public string? UrlFinal { get; init; }

    // Id de la imagen consolidada (null si la carga sigue en progreso)
    public Guid? ImagenId { get; init; }
}
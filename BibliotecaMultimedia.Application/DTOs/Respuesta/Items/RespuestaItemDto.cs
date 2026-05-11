namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Items;

public record RespuestaItemDto
{
    public string Title { get; init; } = string.Empty;
    public DateOnly? ReleaseDate { get; init; }
    public short? Rating { get; init; }
    public bool IsFavorite { get; init; } = false;
    
    public Dictionary<string, object>? Metadata { get; init; }
    
    public string MediaType { get; init; } = string.Empty;
    public string Format { get; init; }  = string.Empty;
    public string? Platform { get; init; }

    public List<string> Genres { get; init; } = new List<string>();
    public List<string> Creators { get; init; } = new List<string>();
};
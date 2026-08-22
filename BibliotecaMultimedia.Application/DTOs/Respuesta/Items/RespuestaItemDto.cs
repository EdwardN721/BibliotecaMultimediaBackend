namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Items;

public record RespuestaItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public DateOnly? ReleaseDate { get; init; }
    public short? Rating { get; init; }
    public bool IsFavorite { get; init; } = false;
    public string? IsbnOrUpc { get; init; }
    
    public string? MainImageUrl { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    
    public Dictionary<string, object>? Metadata { get; init; }
    
    public Guid MediaTypeId { get; init; }
    public Guid FormatId { get; init; }
    public Guid? PlatformId { get; init; }
    public List<Guid> GenreIds { get; init; } = new List<Guid>();
    public List<Guid> CreatorIds { get; init; } = new List<Guid>();

    public string MediaType { get; init; } = string.Empty;
    public string Format { get; init; }  = string.Empty;
    public string? Platform { get; init; }

    public List<string> Genres { get; init; } = new List<string>();
    public List<string> Creators { get; init; } = new List<string>();
};
namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Items;

public record RespuestaItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public DateOnly? ReleaseDate { get; init; }

    // Promedio calculado de las calificaciones personales (UserItem.PersonalRating)
    public double? RatingPromedio { get; init; }

    public string? IsbnOrUpc { get; init; }

    public string? MainImageUrl { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public Dictionary<string, object>? Metadata { get; init; }

    public Guid MediaTypeId { get; init; }
    public List<Guid> FormatIds { get; init; } = new List<Guid>();
    public List<Guid> PlatformIds { get; init; } = new List<Guid>();
    public List<Guid> GenreIds { get; init; } = new List<Guid>();
    public List<Guid> CreatorIds { get; init; } = new List<Guid>();

    public string MediaType { get; init; } = string.Empty;
    public List<string> Formats { get; init; } = new List<string>();
    public List<string> Platforms { get; init; } = new List<string>();

    public List<string> Genres { get; init; } = new List<string>();
    public List<string> Creators { get; init; } = new List<string>();
};

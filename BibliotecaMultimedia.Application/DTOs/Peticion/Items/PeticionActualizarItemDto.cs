namespace BibliotecaMultimedia.Application.DTOs.Peticion.Items;

public record PeticionActualizarItemDto
{
    public string Title { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public DateOnly? ReleaseDate { get; init; }
    public short? Rating { get; init; }
    public bool IsFavorite { get; init; } = false;
    public string? IsbnOrUpc { get; init; }

    public Dictionary<string, object>? Metadata { get; init; }

    public Guid MediaTypeId { get; init; }

    // Un ítem puede existir en varios formatos (Físico + Digital) y varias plataformas
    public List<Guid> FormatIds { get; init; } = new List<Guid>();
    public List<Guid> PlatformIds { get; init; } = new List<Guid>();

    public List<Guid> GenreIds { get; init; } = new List<Guid>();
    public List<Guid> CreatorIds { get; init; } = new List<Guid>();
};

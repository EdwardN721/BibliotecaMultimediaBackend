namespace BibliotecaMultimedia.Application.DTOs.Peticion.Items;

public record PeticionCrearItemDto
{
    public string Title { get; init; } = string.Empty;
    public DateOnly? ReleaseDate { get; init; }
    public short? Rating { get; init; }
    public bool IsFavorite { get; init; } = false;

    public Dictionary<string, object>? Metadata { get; init; }

    public Guid MediaTypeId { get; init; }
    public Guid FormatId { get; init; }
    public Guid? PlatformId { get; init; }

    public List<Guid> GenreIds { get; init; } = new List<Guid>();
    public List<Guid> CreatorIds { get; init; } = new List<Guid>();
};
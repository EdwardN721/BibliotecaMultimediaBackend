namespace BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;

using BibliotecaMultimedia.Domain.Enums;

public record PeticionAgregarABibliotecaDto
{
    public Guid ItemId { get; init; }
    public ConsumptionStatus Status { get; init; } = ConsumptionStatus.Pendiente;
    public string? Progress { get; init; }
    public bool IsFavorite { get; init; } = false;
    public short? PersonalRating { get; init; }
    public string? Review { get; init; }
    public bool IsPrivate { get; init; } = false;
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; init; }

    /// <summary>Formatos en que el usuario posee el título (ej: Físico, Digital).</summary>
    public List<Guid> OwnedFormatIds { get; init; } = new();

    /// <summary>Plataformas/consolas en que el usuario posee el título (ej: PS5, Spotify).</summary>
    public List<Guid> OwnedPlatformIds { get; init; } = new();
};
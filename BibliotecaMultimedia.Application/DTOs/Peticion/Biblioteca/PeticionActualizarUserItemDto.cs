namespace BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;

using BibliotecaMultimedia.Domain.Enums;

public record PeticionActualizarUserItemDto
{
    public ConsumptionStatus? Status { get; init; }
    public string? Progress { get; init; }
    public bool? IsFavorite { get; init; }
    public short? PersonalRating { get; init; }
    public string? Review { get; init; }
    public bool? IsPrivate { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; init; }

    /// <summary>Sincroniza los formatos propios del título (null = no tocar).</summary>
    public List<Guid>? OwnedFormatIds { get; init; }

    /// <summary>Sincroniza las plataformas propias del título (null = no tocar).</summary>
    public List<Guid>? OwnedPlatformIds { get; init; }
};
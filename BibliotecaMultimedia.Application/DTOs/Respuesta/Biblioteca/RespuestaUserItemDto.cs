namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;

using BibliotecaMultimedia.Domain.Enums;

public record RespuestaUserItemDto
{
    public Guid Id { get; init; }
    public Guid ItemId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string MediaType { get; init; } = string.Empty;
    public List<string> Formats { get; init; } = new List<string>();
    public List<string> Platforms { get; init; } = new List<string>();
    public List<string> Genres { get; init; } = new List<string>();
    public List<string> Creators { get; init; } = new List<string>();
    public string? ImageUrl { get; init; }

    /// <summary>Nombres de los formatos en que el usuario posee el título.</summary>
    public List<string> OwnedFormats { get; init; } = new List<string>();
    /// <summary>Ids de los formatos propios del usuario.</summary>
    public List<Guid> OwnedFormatIds { get; init; } = new List<Guid>();
    /// <summary>Nombres de las plataformas/consolas en que el usuario posee el título.</summary>
    public List<string> OwnedPlatforms { get; init; } = new List<string>();
    /// <summary>Ids de las plataformas propias del usuario.</summary>
    public List<Guid> OwnedPlatformIds { get; init; } = new List<Guid>();

    /// <summary>Nombre de la persona que tiene el título prestado ahora mismo (null si no está prestado).</summary>
    public string? PrestamoActivoA { get; init; }

    public ConsumptionStatus Status { get; init; } = ConsumptionStatus.Pendiente;
    public string? Progress { get; init; }
    public bool IsFavorite { get; init; }
    public short? PersonalRating { get; init; }
    public string? Review { get; init; }
    public bool IsPrivate { get; init; }

    public DateTimeOffset? DateAdded { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
};
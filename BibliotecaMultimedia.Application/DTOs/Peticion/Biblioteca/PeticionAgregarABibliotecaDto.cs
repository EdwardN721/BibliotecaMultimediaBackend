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
};
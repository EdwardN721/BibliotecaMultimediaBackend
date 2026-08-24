using BibliotecaMultimedia.Domain.Enums;

namespace BibliotecaMultimedia.Domain.Models;

public class UserItem : BaseEntity
{
    public required Guid UserId { get; set; }
    public User? User { get; set; } 

    public required Guid ItemId { get; set;}
    public Item? Item { get; set; }

    public ConsumptionStatus Status { get; set; } = ConsumptionStatus.Pendiente;
    public string? Progress { get; set; }
    public bool IsFavorite { get; set; } = false;
    public short? PersonalRating { get; set; }
    public string? Review { get; set; }

    public DateTimeOffset? DateAdded { get; set; }
    public DateTimeOffset? StartedAt { get; set; }  
    public DateTimeOffset? FinishedAt { get; set; } 
    public bool IsPrivate { get; set; } = false;

    /// <summary>Formatos en que el usuario posee este título.</summary>
    public ICollection<UserItemFormat>? UserItemFormats { get; set; } = new List<UserItemFormat>();

    /// <summary>Plataformas/consolas en que el usuario posee este título.</summary>
    public ICollection<UserItemPlatform>? UserItemPlatforms { get; set; } = new List<UserItemPlatform>();

    /// <summary>Historial de préstamos de este título (a quién se prestó y cuándo).</summary>
    public ICollection<Prestamo>? Prestamos { get; set; } = new List<Prestamo>();
}
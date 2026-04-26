using System.Text.Json;

namespace BibliotecaMultimedia.Domain.Models;

public class Item : BaseEntity
{
    public required Guid UserId { get; set; }
    public required Guid MediaTypeId  { get; set; }
    public required Guid FormatId  { get; set; }
    public Guid? PlatformId { get; set; }
    
    public required string Title { get; set; } = string.Empty;
    public DateOnly? ReleaseDate { get; set; }
    public short? Rating { get; set; }
    public bool IsFavorite { get; set; } = false;
    
    public JsonDocument? Metadata { get; set; }
    
    public MediaType? MediaType { get; set; }
    public Format? Format { get; set; }
    public Platform? Platform { get; set; }

    public ICollection<ItemCreator> ItemCreators { get; private set; } = new List<ItemCreator>();
    public ICollection<ItemGenre> ItemGenres { get; private set; } = new List<ItemGenre>();
    public ICollection<ItemImage> ItemImages { get; private set; } = new List<ItemImage>();
}
namespace BibliotecaMultimedia.Domain.Models;

public class ItemImage : BaseEntity
{
    public required Guid ItemId { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsPrimary { get; set; } = false;
    
    public Item? Item { get; set; }
}
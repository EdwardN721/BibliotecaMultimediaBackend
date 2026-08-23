namespace BibliotecaMultimedia.Domain.Models;

public class ItemPlatform : BaseEntity
{
    public required Guid ItemId { get; set; }
    public required Guid PlatformId { get; set; }

    public Item? Item { get; set; }
    public Platform? Platform { get; set; }
}

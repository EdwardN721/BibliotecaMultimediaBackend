namespace BibliotecaMultimedia.Domain.Models;

public class ItemFormat : BaseEntity
{
    public required Guid ItemId { get; set; }
    public required Guid FormatId { get; set; }

    public Item? Item { get; set; }
    public Format? Format { get; set; }
}

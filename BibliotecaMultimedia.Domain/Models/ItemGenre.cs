namespace BibliotecaMultimedia.Domain.Models;

public class ItemGenre : BaseEntity
{
    public required Guid ItemId { get; set; }
    public required Guid GenreId { get; set; }
    
    public Item? Item { get; set; }
    public Genre? Genre { get; set; }
}
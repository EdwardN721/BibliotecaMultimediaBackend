namespace BibliotecaMultimedia.Domain.Models;

public class ItemCreator : BaseEntity
{
    public required Guid ItemId { get; set; }
    public required Guid CreatorId { get; set; }
    public required Guid RoleId { get; set; }
    
    public Item? Item { get; set; }
    public Creator? Creator { get; set; }
    public Role? Role { get; set; }
}
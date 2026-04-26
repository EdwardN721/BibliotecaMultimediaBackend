namespace BibliotecaMultimedia.Domain.Models;

public class Creator : BaseEntity
{
    public required string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }

    public ICollection<ItemCreator> ItemCreators { get; private set; } = new List<ItemCreator>();
}
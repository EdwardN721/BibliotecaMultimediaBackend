namespace BibliotecaMultimedia.Domain.Models;

public class Genre : BaseEntity
{
    public required string Name { get; set; } = string.Empty;
}
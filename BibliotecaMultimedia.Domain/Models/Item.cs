using System.Text.Json;

namespace BibliotecaMultimedia.Domain.Models;

public class Item : BaseEntity
{
    public required Guid MediaTypeId  { get; set; }

    public required string Title { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? IsbnOrUpc { get; set; }

    public JsonDocument? Metadata { get; set; }

    public MediaType? MediaType { get; set; }

    // Navegación inversa de la biblioteca de usuarios; se usa para calcular
    // el rating promedio del ítem a partir de las calificaciones personales
    public ICollection<UserItem>? UserItems { get; private set; } = new List<UserItem>();

    public ICollection<ItemCreator>? ItemCreators { get; private set; } = new List<ItemCreator>();
    public ICollection<ItemGenre>? ItemGenres { get; private set; } = new List<ItemGenre>();
    public ICollection<ItemFormat>? ItemFormats { get; private set; } = new List<ItemFormat>();
    public ICollection<ItemPlatform>? ItemPlatforms { get; private set; } = new List<ItemPlatform>();
    public ICollection<ItemImage>? ItemImages { get; private set; } = new List<ItemImage>();
}

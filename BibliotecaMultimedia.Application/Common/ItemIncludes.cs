namespace BibliotecaMultimedia.Application.Common;

/// <summary>
/// Cadenas de includes centralizadas: si se renombra una navegación
/// solo hay que actualizarlas aquí, en un único lugar.
/// </summary>
public static class ItemIncludes
{
    /// <summary>Agregado Item completo (imágenes y registros de biblioteca incluidos).</summary>
    public const string DesdeItem =
        "MediaType,ItemFormats.Format,ItemPlatforms.Platform,ItemGenres.Genre,ItemCreators.Creator,ItemImages,UserItems";

    /// <summary>Agregado UserItem con el Item, sus relaciones y la copia propia (incluidos préstamos activos).</summary>
    public const string DesdeUserItem =
        "Item.MediaType,Item.ItemFormats.Format,Item.ItemPlatforms.Platform,Item.ItemGenres.Genre,Item.ItemCreators.Creator,Item.ItemImages,UserItemFormats.Format,UserItemPlatforms.Platform,Prestamos";
}

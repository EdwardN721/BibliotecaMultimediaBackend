namespace BibliotecaMultimedia.Domain.Models;

/// <summary>
/// Formato(s) en que el usuario posee físicamente/digitalmente el título
/// (ej: Blu-ray, DVD, Digital). Es independiente de los formatos del catálogo del ítem.
/// </summary>
public class UserItemFormat : BaseEntity
{
    public required Guid UserItemId { get; set; }
    public required Guid FormatId { get; set; }

    public UserItem? UserItem { get; set; }
    public Format? Format { get; set; }
}

namespace BibliotecaMultimedia.Domain.Models;

/// <summary>
/// Plataforma(s)/consola(s) en que el usuario posee el título
/// (ej: PS5, Switch, PC, Spotify). Es independiente de las plataformas del catálogo del ítem.
/// </summary>
public class UserItemPlatform : BaseEntity
{
    public required Guid UserItemId { get; set; }
    public required Guid PlatformId { get; set; }

    public UserItem? UserItem { get; set; }
    public Platform? Platform { get; set; }
}

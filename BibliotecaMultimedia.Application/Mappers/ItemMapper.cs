using System.Linq.Expressions;
using System.Text.Json;
using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Items;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class ItemMapper
{
    /// <summary>
    /// DTO intermedio para proyección SQL (evita materializar toda la entidad Item en listas).
    /// </summary>
    public sealed class ProyeccionItemDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Descripcion { get; init; }
        public DateOnly? ReleaseDate { get; init; }
        public double? RatingPromedio { get; init; }
        public string? IsbnOrUpc { get; init; }
        public string? MainImageUrl { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public JsonDocument? Metadata { get; init; }
        public Guid MediaTypeId { get; init; }
        public List<Guid> FormatIds { get; init; } = new();
        public List<Guid> PlatformIds { get; init; } = new();
        public List<Guid> GenreIds { get; init; } = new();
        public List<Guid> CreatorIds { get; init; } = new();
        public string MediaType { get; init; } = string.Empty;
        public List<string> Formats { get; init; } = new();
        public List<string> Platforms { get; init; } = new();
        public List<string> Genres { get; init; } = new();
        public List<string> Creators { get; init; } = new();
    }

    public static Expression<Func<Item, ProyeccionItemDto>> ProyeccionLista()
    {
        return i => new ProyeccionItemDto
        {
            Id = i.Id,
            Title = i.Title,
            Descripcion = i.Descripcion,
            ReleaseDate = i.ReleaseDate,
            RatingPromedio = i.UserItems!
                .Where(ui => ui.PersonalRating != null)
                .Select(ui => (double?)ui.PersonalRating!.Value)
                .Average(),
            IsbnOrUpc = i.IsbnOrUpc,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
            Metadata = i.Metadata,
            MediaType = i.MediaType != null ? i.MediaType.Name : string.Empty,
            MediaTypeId = i.MediaTypeId,
            MainImageUrl = i.ItemImages!
                .OrderBy(im => im.IsPrimary ? 0 : 1)
                .Select(im => im.ImageUrl)
                .FirstOrDefault(),
            Formats = i.ItemFormats!
                .Select(ifm => ifm.Format != null ? ifm.Format.Name : string.Empty)
                .Where(n => n != string.Empty)
                .ToList(),
            Platforms = i.ItemPlatforms!
                .Select(ip => ip.Platform != null ? ip.Platform.Name : string.Empty)
                .Where(n => n != string.Empty)
                .ToList(),
            Genres = i.ItemGenres!
                .Select(ig => ig.Genre != null ? ig.Genre.Name : string.Empty)
                .Where(n => n != string.Empty)
                .ToList(),
            Creators = i.ItemCreators!
                .Select(ic => ic.Creator != null ? ic.Creator.Name : string.Empty)
                .Where(n => n != string.Empty)
                .ToList(),
            FormatIds = i.ItemFormats!
                .Select(ifm => ifm.FormatId)
                .ToList(),
            PlatformIds = i.ItemPlatforms!
                .Select(ip => ip.PlatformId)
                .ToList(),
            GenreIds = i.ItemGenres!
                .Select(ig => ig.GenreId)
                .ToList(),
            CreatorIds = i.ItemCreators!
                .Select(ic => ic.CreatorId)
                .ToList(),
        };
    }

    public static RespuestaItemDto MapProyeccionToDto(ProyeccionItemDto proyeccion)
    {
        return new RespuestaItemDto
        {
            Id = proyeccion.Id,
            Title = proyeccion.Title,
            Descripcion = proyeccion.Descripcion,
            ReleaseDate = proyeccion.ReleaseDate,
            RatingPromedio = proyeccion.RatingPromedio,
            IsbnOrUpc = proyeccion.IsbnOrUpc,
            CreatedAt = proyeccion.CreatedAt,
            UpdatedAt = proyeccion.UpdatedAt,
            MainImageUrl = proyeccion.MainImageUrl,
            Metadata = proyeccion.Metadata != null
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(proyeccion.Metadata.RootElement.GetRawText())
                : null,
            MediaType = proyeccion.MediaType,
            Formats = proyeccion.Formats,
            Platforms = proyeccion.Platforms,
            Genres = proyeccion.Genres,
            Creators = proyeccion.Creators,
            MediaTypeId = proyeccion.MediaTypeId,
            FormatIds = proyeccion.FormatIds,
            PlatformIds = proyeccion.PlatformIds,
            GenreIds = proyeccion.GenreIds,
            CreatorIds = proyeccion.CreatorIds,
        };
    }

    public static IEnumerable<RespuestaItemDto> MapProyeccionToDto(this IEnumerable<ProyeccionItemDto>? proyecciones)
    {
        return proyecciones?.Select(MapProyeccionToDto) ?? Enumerable.Empty<RespuestaItemDto>();
    }

    public static Item MapToEntity(this PeticionCrearItemDto itemDto, Guid currentUserId)
    {
        return new Item
        {
            Title = itemDto.Title,
            Descripcion = itemDto.Descripcion,
            ReleaseDate = itemDto.ReleaseDate,
            IsbnOrUpc = itemDto.IsbnOrUpc,
            Metadata = itemDto.Metadata is null
                ? null
                : JsonDocument.Parse(JsonSerializer.Serialize(itemDto.Metadata)),
            MediaTypeId = itemDto.MediaTypeId,
        };
    }

    public static RespuestaItemDto MapToDto(this Item item)
    {
        return new RespuestaItemDto
        {
            Id = item.Id,
            Title = item.Title,
            Descripcion = item.Descripcion,
            ReleaseDate = item.ReleaseDate,
            RatingPromedio = CalcularRatingPromedio(item),
            IsbnOrUpc = item.IsbnOrUpc,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,

            Metadata = item.Metadata != null
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(item.Metadata.RootElement.GetRawText())
                : null,

            MediaType = item.MediaType?.Name ?? string.Empty,
            MediaTypeId = item.MediaTypeId,
            MainImageUrl = ObtenerImagenPrincipal(item),

            Formats = item.ItemFormats?.Select(f => f.Format?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
            Platforms = item.ItemPlatforms?.Select(p => p.Platform?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
            Genres = item.ItemGenres?.Select(ig => ig.Genre?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
            Creators = item.ItemCreators?.Select(ic => ic.Creator?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
            FormatIds = item.ItemFormats?.Select(f => f.FormatId).ToList() ?? new List<Guid>(),
            PlatformIds = item.ItemPlatforms?.Select(p => p.PlatformId).ToList() ?? new List<Guid>(),
            GenreIds = item.ItemGenres?.Select(ig => ig.GenreId).ToList() ?? new List<Guid>(),
            CreatorIds = item.ItemCreators?.Select(ic => ic.CreatorId).ToList() ?? new List<Guid>()
        };
    }

    public static IEnumerable<RespuestaItemDto> MapToDto(this IEnumerable<Item>? items)
    {
        return items?.Select(MapToDto) ?? Enumerable.Empty<RespuestaItemDto>();
    }

    public static void UpdateEntity(this Item item, PeticionActualizarItemDto itemDto)
    {
        item.Title = itemDto.Title;
        item.Descripcion = itemDto.Descripcion;
        item.ReleaseDate = itemDto.ReleaseDate;
        item.IsbnOrUpc = itemDto.IsbnOrUpc;
        item.Metadata = itemDto.Metadata is null
            ? null
            : JsonDocument.Parse(JsonSerializer.Serialize(itemDto.Metadata));
        item.MediaTypeId = itemDto.MediaTypeId;
    }

    private static string? ObtenerImagenPrincipal(Item item)
    {
        if (item.ItemImages is null || item.ItemImages.Count == 0)
        {
            return null;
        }

        return item.ItemImages.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
               ?? item.ItemImages.First().ImageUrl;
    }

    private static double? CalcularRatingPromedio(Item item)
    {
        List<double> calificaciones = (item.UserItems ?? new List<UserItem>())
            .Where(ui => ui.PersonalRating.HasValue)
            .Select(ui => (double)ui.PersonalRating!.Value)
            .ToList();

        return calificaciones.Count > 0 ? calificaciones.Average() : null;
    }
}

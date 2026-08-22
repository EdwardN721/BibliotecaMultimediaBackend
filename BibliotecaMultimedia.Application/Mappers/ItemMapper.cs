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
        public short? Rating { get; init; }
        public bool IsFavorite { get; init; }
        public string? IsbnOrUpc { get; init; }
        public string? MainImageUrl { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public JsonDocument? Metadata { get; init; }
        public Guid MediaTypeId { get; init; }
        public Guid FormatId { get; init; }
        public Guid? PlatformId { get; init; }
        public List<Guid> GenreIds { get; init; } = new();
        public List<Guid> CreatorIds { get; init; } = new();
        public string MediaType { get; init; } = string.Empty;
        public string Format { get; init; } = string.Empty;
        public string? Platform { get; init; }
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
            Rating = i.Rating,
            IsFavorite = i.IsFavorite,
            IsbnOrUpc = i.IsbnOrUpc,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
            Metadata = i.Metadata,
            MediaType = i.MediaType != null ? i.MediaType.Name : string.Empty,
            Format = i.Format != null ? i.Format.Name : string.Empty,
            Platform = i.Platform != null ? i.Platform.Name : null,
            MediaTypeId = i.MediaTypeId,
            FormatId = i.FormatId,
            PlatformId = i.PlatformId,
            MainImageUrl = i.ItemImages!
                .OrderBy(im => im.IsPrimary ? 0 : 1)
                .Select(im => im.ImageUrl)
                .FirstOrDefault(),
            Genres = i.ItemGenres!
                .Select(ig => ig.Genre != null ? ig.Genre.Name : string.Empty)
                .Where(n => n != string.Empty)
                .ToList(),
            Creators = i.ItemCreators!
                .Select(ic => ic.Creator != null ? ic.Creator.Name : string.Empty)
                .Where(n => n != string.Empty)
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
            Rating = proyeccion.Rating,
            IsFavorite = proyeccion.IsFavorite,
            IsbnOrUpc = proyeccion.IsbnOrUpc,
            CreatedAt = proyeccion.CreatedAt,
            UpdatedAt = proyeccion.UpdatedAt,
            MainImageUrl = proyeccion.MainImageUrl,
            Metadata = proyeccion.Metadata != null
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(proyeccion.Metadata.RootElement.GetRawText())
                : null,
            MediaType = proyeccion.MediaType,
            Format = proyeccion.Format,
            Platform = proyeccion.Platform,
            Genres = proyeccion.Genres,
            Creators = proyeccion.Creators,
            MediaTypeId = proyeccion.MediaTypeId,
            FormatId = proyeccion.FormatId,
            PlatformId = proyeccion.PlatformId,
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
            Rating = itemDto.Rating,
            IsFavorite = itemDto.IsFavorite,
            IsbnOrUpc = itemDto.IsbnOrUpc,
            Metadata = itemDto.Metadata is null
                ? null
                : JsonDocument.Parse(JsonSerializer.Serialize(itemDto.Metadata)),
            MediaTypeId = itemDto.MediaTypeId,
            FormatId = itemDto.FormatId,
            PlatformId = itemDto.PlatformId,
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
            Rating = item.Rating,
            IsFavorite = item.IsFavorite,
            IsbnOrUpc = item.IsbnOrUpc,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            
            Metadata = item.Metadata != null 
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(item.Metadata.RootElement.GetRawText()) 
                : null,
            
            MediaType = item.MediaType?.Name ?? string.Empty,
            Format = item.Format?.Name ?? string.Empty,
            Platform = item.Platform?.Name,
            MediaTypeId = item.MediaTypeId,
            FormatId = item.FormatId,
            PlatformId = item.PlatformId,
            MainImageUrl = ObtenerImagenPrincipal(item),
            
            Genres = item.ItemGenres?.Select(ig => ig.Genre?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
            Creators = item.ItemCreators?.Select(ic => ic.Creator?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
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
        item.Rating = itemDto.Rating;
        item.IsFavorite = itemDto.IsFavorite;
        item.IsbnOrUpc = itemDto.IsbnOrUpc;
        item.Metadata = itemDto.Metadata is null
            ? null
            : JsonDocument.Parse(JsonSerializer.Serialize(itemDto.Metadata));
        item.MediaTypeId = itemDto.MediaTypeId;
        item.FormatId = itemDto.FormatId;
        item.PlatformId = itemDto.PlatformId;
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
}
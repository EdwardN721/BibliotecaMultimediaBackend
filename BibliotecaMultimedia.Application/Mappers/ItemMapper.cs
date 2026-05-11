using System.Text.Json;
using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Items;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class ItemMapper
{
    public static Item MapToEntity(this PeticionCrearITemDto itemDto, Guid currentUserId)
    {
        return new Item
        {
            UserId = currentUserId,
            Title = itemDto.Title,
            ReleaseDate = itemDto.ReleaseDate,
            Rating = itemDto.Rating,
            IsFavorite = itemDto.IsFavorite,
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
            Title = item.Title,
            ReleaseDate = item.ReleaseDate,
            Rating = item.Rating,
            IsFavorite = item.IsFavorite,
            Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(item.Metadata)),
        };
    }

    public static IEnumerable<RespuestaItemDto> MapToDto(this IEnumerable<Item>? items)
    {
        return items?.Select(MapToDto) ?? Enumerable.Empty<RespuestaItemDto>();
    }

    public static void UpadteEntity(this Item item, PeticionActualizarItemDto itemDto)
    {
        item.Title = itemDto.Title;
        item.ReleaseDate = itemDto.ReleaseDate;
        item.Rating = itemDto.Rating;
        item.IsFavorite = itemDto.IsFavorite;
        item.Metadata = itemDto.Metadata is null
            ? null
            : JsonDocument.Parse(JsonSerializer.Serialize(itemDto.Metadata));
        item.MediaTypeId = itemDto.MediaTypeId;
        item.FormatId = itemDto.FormatId;
        item.PlatformId = itemDto.PlatformId;
    }
}
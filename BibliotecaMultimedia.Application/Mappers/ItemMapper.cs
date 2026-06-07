using System.Text.Json;
using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Items;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class ItemMapper
{
    public static Item MapToEntity(this PeticionCrearItemDto itemDto, Guid currentUserId)
    {
        return new Item
        {
            Title = itemDto.Title,
            ReleaseDate = itemDto.ReleaseDate,
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
            ReleaseDate = item.ReleaseDate,
            
            Metadata = item.Metadata != null 
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(item.Metadata.RootElement.GetRawText()) 
                : null,
            
            MediaType = item.MediaType?.Name ?? string.Empty,
            Format = item.Format?.Name ?? string.Empty,
            Platform = item.Platform?.Name,
            
            Genres = item.ItemGenres?.Select(ig => ig.Genre?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
            Creators = item.ItemCreators?.Select(ic => ic.Creator?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>()
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
        item.Metadata = itemDto.Metadata is null
            ? null
            : JsonDocument.Parse(JsonSerializer.Serialize(itemDto.Metadata));
        item.MediaTypeId = itemDto.MediaTypeId;
        item.FormatId = itemDto.FormatId;
        item.PlatformId = itemDto.PlatformId;
    }
}
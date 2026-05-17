using BibliotecaMultimedia.Application.DTOs.Peticion.Images;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Imagenes;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class ImagenMapper
{
    public static ItemImage MapToEntity(this PeticionAgregarImagenDto imagenDto)
    {
        return new ItemImage
        {
            ItemId = imagenDto.ItemId,
            ImageUrl = imagenDto.ImageUrl,
            IsPrimary = imagenDto.IsPrimary,
        };
    }

    public static RespuestaImagenDto MapToDto(this ItemImage image)
    {
        return new RespuestaImagenDto
        {
            Id = image.ItemId,
            ItemId = image.ItemId,
            ImageUrl = image.ImageUrl,
            IsPrimary = image.IsPrimary,
            CreatedAt = image.CreatedAt,
            UpdatedAt = image.UpdatedAt
        };
    }

    public static IEnumerable<RespuestaImagenDto> MapToDto(this IEnumerable<ItemImage>? imagenes)
    {
        return imagenes?.Select(MapToDto) ?? Enumerable.Empty<RespuestaImagenDto>();
    }

    public static void UpdateEntity(this ItemImage image, PeticionActualizarImagenDto entity)
    {
        image.ItemId = entity.ItemId;
        image.ImageUrl = entity.ImageUrl;
        image.IsPrimary = entity.IsPrimary;
    }
}
using BibliotecaMultimedia.Application.DTOs.Peticion.MediaType;
using BibliotecaMultimedia.Application.DTOs.Respuesta.MediaType;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class MediaTypeMapper
{
    public static MediaType MapToEntity(this PeticionCrearMediaTypeDto dto)
    {
        return new MediaType
        {
            Name = dto.Nombre,
        };
    }

    public static RespuestaMediaTypeDto MapToDto(this MediaType entity)
    {
        return new RespuestaMediaTypeDto()
        {
            Id = entity.Id,
            Nombre = entity.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    public static IEnumerable<RespuestaMediaTypeDto> MapToDto(this IEnumerable<MediaType>? entities)
    {
        return entities?.Select(MapToDto) ?? Enumerable.Empty<RespuestaMediaTypeDto>();
    }

    public static void UpdateEntity(this MediaType entity, PeticionActualizarMediaTypeDto dtoEntity)
    {
        entity.Name = dtoEntity.Nombre;
    }
}
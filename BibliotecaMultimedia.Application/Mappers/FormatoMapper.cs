using BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Formatos;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class FormatoMapper
{
    public static Format MapToEntity(this PeticionCrearFormatoDto dto)
    {
        return new Format
        {
            Name = dto.Nombre
        };
    }

    public static RespuestaFormatoDto MapToDto(this Format entity)
    {
        return new RespuestaFormatoDto
        {
            Id = entity.Id,
            Nombre = entity.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static IEnumerable<RespuestaFormatoDto> MapToDto(this IEnumerable<Format>? entities)
    {
        return entities?.Select(MapToDto) ?? Enumerable.Empty<RespuestaFormatoDto>();
    }

    public static void UpdateEntity(this Format format, PeticionActualizarFormatoDto entity)
    {
        format.Name = entity.Nombre;
    }
}
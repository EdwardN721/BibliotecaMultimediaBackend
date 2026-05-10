using BibliotecaMultimedia.Application.DTOs.Peticion.Creador;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Creador;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class CreadorMapper
{
    public static Creator MapToEntity(this PeticionCrearCreadorDto creadorDto)
    {
        return new Creator
        {
            Name = creadorDto.Nombre,
            Bio = creadorDto.Biografia,
        };
    }

    public static RespuestaCreadorDto MapToDto(this Creator creator)
    {
        return new RespuestaCreadorDto
        {
            Id = creator.Id,
            Nombre = creator.Name,
            Biografia = creator.Bio,
            CreatedAt = creator.CreatedAt,
            UpdatedAt = creator.UpdatedAt
        };
    }

    public static IEnumerable<RespuestaCreadorDto> MapToDto(this IEnumerable<Creator>? creators)
    {
        return creators?.Select(MapToDto) ?? Enumerable.Empty<RespuestaCreadorDto>();
    }

    public static void UpdateEntity(this Creator creator, PeticionActualizarCreadorDto updateCreator)
    {
        creator.Name = updateCreator.Nombre;
        creator.Bio = updateCreator.Biografia;
    }
}
using BibliotecaMultimedia.Application.DTOs.Peticion.Plataformas;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Plataformas;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class PlataformaMapper
{
    public static Platform MapToEntity(this PeticionCrearPlataformaDto dto)
    {
        return new Platform
        {
            Name = dto.Nombre
        };
    }

    public static void UpdateEntity(this Platform platform, PeticionActualizarPlataformaDto dto)
    {
        platform.Name = dto.Nombre;
    }

    public static RespuestaPlataformaDto MapToDto(this Platform platform)
    {
        return new RespuestaPlataformaDto
        {
            Id = platform.Id,
            Nombre = platform.Name,
            CreatedAt = platform.CreatedAt,
            UpdatedAt = platform.UpdatedAt
        };
    }

    public static IEnumerable<RespuestaPlataformaDto> MapToDto(this IEnumerable<Platform>? platforms)
    {
        return platforms?.Select(MapToDto) ?? Enumerable.Empty<RespuestaPlataformaDto>();
    }
}

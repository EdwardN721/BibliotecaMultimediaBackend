using BibliotecaMultimedia.Application.DTOs.Peticion.Catalogos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Catalogos;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class CatalogoMapper
{
  public static Genre MapToEntity(this PeticionCrearGeneroDto generoDto)
  {
    return new Genre
    {
      Name = generoDto.Name,
      Description = generoDto.Description
    };
  }

  public static void UpdateEntity(this Genre genero, PeticionActualizarGeneroDto generoDto)
  {
    if (generoDto.Name != null) genero.Name = generoDto.Name;
    if (generoDto.Description != null) genero.Description = generoDto.Description;
  }

  public static RespuestaGeneroDto MapToDto(this Genre genre)
  {
    return new RespuestaGeneroDto
    {
      Id = genre.Id,
      Name = genre.Name,
      Description = genre.Description
    };
  }

  public static IEnumerable<RespuestaGeneroDto> MapToDto(this IEnumerable<Genre>? genres)
  {
    return genres?.Select(MapToDto) ?? Enumerable.Empty<RespuestaGeneroDto>();
  }
}
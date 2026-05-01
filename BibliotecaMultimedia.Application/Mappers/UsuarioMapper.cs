using BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Usuarios;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class UsuarioMapper
{
    public static User ToEntity(this PeticionCrearUsuarioDto dto)
    {
        return new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            Nombre = dto.Nombre,
            PrimerApellido = dto.PrimerApellido,
            SegundoApellido = dto.SegundoApellido,
            PhoneNumber = dto.PhoneNumber,
        };
    }

    public static RespuestaUsuarioDto MapToDto(this User user)
    {
        return new RespuestaUsuarioDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Nombre = user.Nombre,
            PrimerApellido = user.PrimerApellido,
            SegundoApellido = user.SegundoApellido,
            PhoneNumber = user.PhoneNumber
        };
    }

    public static IEnumerable<RespuestaUsuarioDto> MapToDto(this IEnumerable<User>? users)
    {
        return users?.Select(MapToDto) ?? Enumerable.Empty<RespuestaUsuarioDto>();
    }
    
    public static void UpdateEntity(this User user, PeticionActualizarUsuarioDto dto)
    {
        if (dto.Nombre != null) user.Nombre = dto.Nombre;
        if (dto.PrimerApellido != null) user.PrimerApellido = dto.PrimerApellido;
        if (dto.SegundoApellido != null) user.SegundoApellido = dto.SegundoApellido;
        if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
    }
}
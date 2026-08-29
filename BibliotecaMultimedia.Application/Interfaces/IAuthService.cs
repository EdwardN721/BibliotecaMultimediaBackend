using BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Usuarios;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(PeticionCrearUsuarioDto peticion, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto peticion, CancellationToken cancellationToken = default);

    /// <summary>Devuelve el perfil del usuario autenticado.</summary>
    Task<RespuestaUsuarioDto> ObtenerPerfilAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Actualiza los datos editables del perfil del usuario autenticado.</summary>
    Task<RespuestaUsuarioDto> ActualizarPerfilAsync(Guid userId, PeticionActualizarUsuarioDto peticion, CancellationToken cancellationToken = default);
}
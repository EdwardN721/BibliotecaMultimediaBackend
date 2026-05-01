using BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Usuarios;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(PeticionCrearUsuarioDto peticion);
    Task<AuthResponseDto> LoginAsync(LoginDto peticion);
}
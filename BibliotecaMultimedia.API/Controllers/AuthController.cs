using BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Usuarios;
using BibliotecaMultimedia.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMultimedia.API.Controllers;

/// <summary>
/// Controller que administra la autenticacion
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// Registrar a un usuario
    /// </summary>
    /// <param name="peticion">Dto que registrará al usuario.</param>
    /// <returns>Código de creación y respuesta.</returns>
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] PeticionCrearUsuarioDto peticion)
    {
        AuthResponseDto response = await _authService.RegisterAsync(peticion);
        return StatusCode(StatusCodes.Status201Created, response);
    }
    
    /// <summary>
    /// Servicio que registrara el login
    /// </summary>
    /// <param name="request">Parametros para iniciar la sesión.</param>
    /// <returns>Retorna Token y datos de usuarios.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        AuthResponseDto response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}
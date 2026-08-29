using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Usuarios;


namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controller que administra la autenticacion
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
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
    public async Task<IActionResult> Registrar([FromBody] PeticionCrearUsuarioDto peticion, CancellationToken cancellationToken)
    {
        AuthResponseDto response = await _authService.RegisterAsync(peticion, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }
    
    /// <summary>
    /// Servicio que registrara el login
    /// </summary>
    /// <param name="request">Parametros para iniciar la sesión.</param>
    /// <returns>Retorna Token y datos de usuarios.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request, CancellationToken cancellationToken)
    {
        AuthResponseDto response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Devuelve el perfil del usuario autenticado.
    /// </summary>
    [Authorize]
    [HttpGet("perfil")]
    [ProducesResponseType(typeof(RespuestaUsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPerfil(CancellationToken cancellationToken)
    {
        Guid userId = ObtenerUserId();
        RespuestaUsuarioDto perfil = await _authService.ObtenerPerfilAsync(userId, cancellationToken);
        return Ok(perfil);
    }

    /// <summary>
    /// Actualiza los datos editables del perfil del usuario autenticado.
    /// </summary>
    [Authorize]
    [HttpPut("perfil")]
    [ProducesResponseType(typeof(RespuestaUsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarPerfil([FromBody] PeticionActualizarUsuarioDto peticion, CancellationToken cancellationToken)
    {
        Guid userId = ObtenerUserId();
        RespuestaUsuarioDto perfil = await _authService.ActualizarPerfilAsync(userId, peticion, cancellationToken);
        return Ok(perfil);
    }

    private Guid ObtenerUserId()
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new UnauthorizedAppException("No se pudo identificar al usuario autenticado.");
        }
        return userId;
    }
}
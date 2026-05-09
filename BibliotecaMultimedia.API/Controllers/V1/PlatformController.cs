using Asp.Versioning;
using BibliotecaMultimedia.Application.DTOs.Peticion.Plataformas;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Plataformas;
using BibliotecaMultimedia.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra las plataformas
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
public class PlatformController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlatformController(IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService ?? throw new ArgumentNullException(nameof(plataformaService));
    }

    /// <summary>
    /// Obtener todas las plataformas
    /// </summary>
    /// <returns>Lista de plataformas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RespuestaPlataformaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodasPlataformas()
    {
        IEnumerable<RespuestaPlataformaDto> plataformas = await _plataformaService.ObtenerPlataformas();
        return Ok(plataformas);
    }

    /// <summary>
    /// Obtener una plataforma por su Id
    /// </summary>
    /// <param name="id">Id de la plataforma</param>
    /// <returns>Plataforma a buscar</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RespuestaPlataformaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPlataformaPorId([FromRoute] Guid id)
    {
        RespuestaPlataformaDto plataforma = await _plataformaService.ObtenerPlataformaPorId(id);
        return Ok(plataforma);
    }

    /// <summary>
    /// Agregar una plataforma
    /// </summary>
    /// <param name="plataformaDto">Información de la plataforma</param>
    /// <returns>Plataforma agregada</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RespuestaPlataformaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarPlataforma([FromBody] PeticionCrearPlataformaDto plataformaDto)
    {
        RespuestaPlataformaDto plataformaNueva = await _plataformaService.AgregarPlataforma(plataformaDto);
        
        return CreatedAtAction(nameof(ObtenerPlataformaPorId), new  { id = plataformaNueva.Id }, plataformaNueva);
    }

    /// <summary>
    /// Actualiza una plataforma
    /// </summary>
    /// <param name="id">Id de la plataforma a actualizar</param>
    /// <param name="plataformaDto">Informacion a actualizar</param>
    /// <returns>Estado de la actualización</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarPlataforma([FromRoute] Guid id,
        PeticionActualizarPlataformaDto plataformaDto)
    {
        await _plataformaService.ActualizarPlataforma(id, plataformaDto);
        return NoContent();
    }

    /// <summary>
    /// Eliminar una plataforma
    /// </summary>
    /// <param name="id">Id de la plataforma a eliminar</param>
    /// <returns>Estado de la eliminación</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarPlataforma([FromRoute] Guid id)
    {
        await _plataformaService.EliminarPlataforma(id);
        return NoContent();
    }
}
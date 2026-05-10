using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Formatos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra los formatos
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
public class FormatController : ControllerBase
{
    private readonly IFormatoService _formatoService;

    public FormatController(IFormatoService formatoService)
    {
        _formatoService = formatoService ?? throw new ArgumentNullException(nameof(formatoService));
    }
    
    /// <summary>
    /// Obtener plataformas de forma paginada y con filtros
    /// </summary>
    /// <param name="filtroFormato">Filtro para la paginacion</param>
    /// <param name="cancellation">Token de cancelacion</param>
    /// <returns>Respuesta páginada.</returns>
    [HttpGet("paginado")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespuestaPaginada<RespuestaFormatoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerFormatosPaginados([FromQuery] FiltroFormato filtroFormato,
        CancellationToken cancellation)
    {
        RespuestaPaginada<RespuestaFormatoDto> resultado = await _formatoService.ObtenerFormatosPaginados(filtroFormato, cancellation);
        
        var metadataJson = JsonSerializer.Serialize(resultado.Metadata);
        
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Pagination");
        Response.Headers.Append("X-Pagination", metadataJson);
        
        return Ok(resultado.Registros);
    }
    
    /// <summary>
    /// Obtener todos los formatos
    /// </summary>
    /// <returns>Lista de formatos</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RespuestaFormatoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosFormatos()
    {
        IEnumerable<RespuestaFormatoDto> formatos = await _formatoService.ObtenerFormatos();
        return Ok(formatos);
    }

    /// <summary>
    /// Obtener un formato por su Id
    /// </summary>
    /// <param name="id">Id del formato.</param>
    /// <returns>Formato a buscar.</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespuestaFormatoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerFormatoPorId([FromRoute] Guid id)
    {
        RespuestaFormatoDto formato = await _formatoService.ObtenerFormatoPorId(id);
        return Ok(formato);
    }

    /// <summary>
    /// Agregar un formato
    /// </summary>
    /// <param name="formatoDto">Información del formato.</param>
    /// <returns>Formato agregado</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RespuestaFormatoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarFormato([FromBody] PeticionCrearFormatoDto formatoDto)
    {
        RespuestaFormatoDto formatoNuevo = await _formatoService.AgregarFormato(formatoDto);
        
        return CreatedAtAction(nameof(ObtenerFormatoPorId), new  { id = formatoNuevo.Id }, formatoNuevo);
    }

    /// <summary>
    /// Actualiza un formato
    /// </summary>
    /// <param name="id">Id del formato a actualizar.</param>
    /// <param name="formatoDto">Informacion a actualizar</param>
    /// <returns>Estado de la actualización</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarFormato([FromRoute] Guid id,
        PeticionActualizarFormatoDto formatoDto)
    {
        await _formatoService.ActualizarFormato(id, formatoDto);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un formato
    /// </summary>
    /// <param name="id">Id del formato a eliminar</param>
    /// <returns>Estado de la eliminación</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarFormato([FromRoute] Guid id)
    {
        await _formatoService.EliminarFormato(id);
        return NoContent();
    }
}
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMultimedia.API.Extensions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.Creador;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Creador;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra los creadores
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
public class CreatorController : ControllerBase
{
    private readonly ICreadorService _creadorService;

    public CreatorController(ICreadorService creadorService)
    {
        _creadorService = creadorService ?? throw new ArgumentNullException(nameof(creadorService));
    }
    
    /// <summary>
    /// Obtener creadores de forma paginada y con filtros
    /// </summary>
    /// <param name="filtroCreador">Filtro para la paginacion</param>
    /// <param name="cancellation">Token de cancelacion</param>
    /// <returns>Respuesta páginada.</returns>
    [HttpGet("paginado")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RespuestaCreadorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerCreadoresPaginado([FromQuery] FiltroCreador filtroCreador,
        CancellationToken cancellation)
    {
        RespuestaPaginada<RespuestaCreadorDto> resultado = await _creadorService.ObtenerCreadoresPaginado(filtroCreador, cancellation);

        PaginacionHeaderHelper.EscribirMetadataPaginacion(Response, resultado.Metadata);

        return Ok(resultado.Registros);
    }
    
    /// <summary>
    /// Obtener todos los creadores
    /// </summary>
    /// <returns>Lista de creadores</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RespuestaCreadorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosCreadores()
    {
        IEnumerable<RespuestaCreadorDto> creadores = await _creadorService.ObtenerCreadores();
        return Ok(creadores);
    }

    /// <summary>
    /// Obtener un creador por su Id
    /// </summary>
    /// <param name="id">Id del creador.</param>
    /// <returns>Creador a buscar.</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespuestaCreadorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerCreadorPorId([FromRoute] Guid id)
    {
        RespuestaCreadorDto creador = await _creadorService.ObtenerCreadorPorId(id);
        return Ok(creador);
    }

    /// <summary>
    /// Agregar un creador
    /// </summary>
    /// <param name="creadorDto">Información del creador.</param>
    /// <returns>creador agregado</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RespuestaCreadorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarCreador([FromBody] PeticionCrearCreadorDto creadorDto)
    {
        RespuestaCreadorDto creadorNuevo = await _creadorService.AgregarCreador(creadorDto);
        
        return CreatedAtAction(nameof(ObtenerCreadorPorId), new  { id = creadorNuevo.Id }, creadorNuevo);
    }

    /// <summary>
    /// Actualiza un creador
    /// </summary>
    /// <param name="id">Id del creador a actualizar.</param>
    /// <param name="creadorDto">Informacion a actualizar</param>
    /// <returns>Estado de la actualización</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarCreador([FromRoute] Guid id,
        PeticionActualizarCreadorDto creadorDto)
    {
        await _creadorService.ActualizarCreador(id, creadorDto);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un creador
    /// </summary>
    /// <param name="id">Id del creador a eliminar</param>
    /// <returns>Estado de la eliminación</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarCreador([FromRoute] Guid id)
    {
        await _creadorService.EliminarCreador(id);
        return NoContent();
    }
}
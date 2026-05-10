using Asp.Versioning;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.MediaType;
using BibliotecaMultimedia.Application.DTOs.Respuesta.MediaType;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra los media types
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
public class MediaTypeController : ControllerBase
{
    private readonly IMediaTypeService _mediaTypeService;

    public MediaTypeController(IMediaTypeService mediaTypeService)
    {
        _mediaTypeService = mediaTypeService ?? throw new ArgumentNullException(nameof(mediaTypeService));
    }
    
    /// <summary>
    /// Obtener MediaTypes de forma paginada y con filtros
    /// </summary>
    /// <param name="filtroMediaType">Filtro para la paginacion</param>
    /// <param name="cancellation">Token de cancelacion</param>
    /// <returns>Respuesta páginada.</returns>
    [HttpGet("paginado")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespuestaPaginada<RespuestaMediaTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPlataformasPaginadas([FromQuery] FiltroMediaType filtroMediaType,
        CancellationToken cancellation)
    {
        RespuestaPaginada<RespuestaMediaTypeDto> resultado = await _mediaTypeService.ObtenerMediaTypePaginado(filtroMediaType, cancellation);
        
        var metadataJson = JsonSerializer.Serialize(resultado.Metadata);
        
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Pagination");
        Response.Headers.Append("X-Pagination", metadataJson);
        
        return Ok(resultado.Registros);
    }

    /// <summary>
    /// Obtener todos los media types
    /// </summary>
    /// <returns>Lista de Media Types</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RespuestaMediaTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerMediaTypes()
    {
        IEnumerable<RespuestaMediaTypeDto> mediaTypes = await _mediaTypeService.ObtenerMediaTypeTodos();
        return Ok(mediaTypes);
    }

    /// <summary>
    /// Obtener un medio
    /// </summary>
    /// <param name="id">Id del medio a traer</param>
    /// <returns>Un Tipo de medio</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespuestaMediaTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerMediaTypePorId([FromRoute] Guid id)
    {
        RespuestaMediaTypeDto mediaType = await _mediaTypeService.ObtenerMediaTypePorId(id);
        return Ok(mediaType);
    }

    /// <summary>
    /// Agregar un tipo medio
    /// </summary>
    /// <param name="mediaTypeDto">Información del tipo de medio.</param>
    /// <returns>Tipo de medio recien creado.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RespuestaMediaTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearMediaType([FromBody] PeticionCrearMediaTypeDto mediaTypeDto)
    {
        RespuestaMediaTypeDto mediaType = await _mediaTypeService.AgregarMediaType(mediaTypeDto);
        return CreatedAtAction(nameof(ObtenerMediaTypePorId), new { id = mediaType.Id }, mediaType);
    }

    /// <summary>
    /// Actualizar un tipo de medio
    /// </summary>
    /// <param name="id">Id del medio a actualizar.</param>
    /// <param name="mediaTypeDto">información a actualizar.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActualizarMediaType([FromRoute] Guid id,
        [FromBody] PeticionActualizarMediaTypeDto mediaTypeDto)
    {
        await _mediaTypeService.ActualizarMediaType(id, mediaTypeDto);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un medio
    /// </summary>
    /// <param name="id">Id del medio a eliminar.</param>
    /// <returns>Estado de la eliminacion.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarMediaType([FromRoute] Guid id)
    {
        await _mediaTypeService.EliminarMediaType(id);
        return NoContent();
    }
}
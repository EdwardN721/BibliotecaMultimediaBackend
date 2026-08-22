using System.Security.Claims;
using System.Text.Json;
using Asp.Versioning;
using BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra la biblioteca personal del usuario autenticado.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class BibliotecaController : ControllerBase
{
    private readonly IBibliotecaService _bibliotecaService;

    public BibliotecaController(IBibliotecaService bibliotecaService)
    {
        _bibliotecaService = bibliotecaService ?? throw new ArgumentNullException(nameof(bibliotecaService));
    }

    /// <summary>
    /// Obtener la biblioteca del usuario autenticado de forma paginada y con filtros.
    /// </summary>
    [HttpGet("paginado")]
    [ProducesResponseType(typeof(IEnumerable<RespuestaUserItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerBibliotecaPaginado([FromQuery] FiltroBiblioteca filtro,
        CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        RespuestaPaginada<RespuestaUserItemDto> resultado =
            await _bibliotecaService.ObtenerBibliotecaPaginado(userId, filtro, cancellation);

        string metadataJson = JsonSerializer.Serialize(resultado.Metadata);
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Pagination");
        Response.Headers.Append("X-Pagination", metadataJson);

        return Ok(resultado.Registros);
    }

    /// <summary>
    /// Obtener estadísticas agregadas de la biblioteca del usuario autenticado.
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(RespuestaBibliotecaStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerStats(CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        RespuestaBibliotecaStatsDto stats = await _bibliotecaService.ObtenerStats(userId, cancellation);
        return Ok(stats);
    }

    /// <summary>
    /// Obtener un elemento específico de la biblioteca del usuario autenticado.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RespuestaUserItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerItemDeBiblioteca([FromRoute] Guid id, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        RespuestaUserItemDto resultado = await _bibliotecaService.ObtenerItemDeBiblioteca(userId, id, cancellation);
        return Ok(resultado);
    }

    /// <summary>
    /// Agregar un ítem a la biblioteca del usuario autenticado.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RespuestaUserItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarABiblioteca([FromBody] PeticionAgregarABibliotecaDto dto,
        CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        RespuestaUserItemDto resultado = await _bibliotecaService.AgregarABiblioteca(userId, dto, cancellation);
        return CreatedAtAction(nameof(ObtenerItemDeBiblioteca), new { id = resultado.Id }, resultado);
    }

    /// <summary>
    /// Actualizar un elemento de la biblioteca del usuario autenticado.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActualizarItemDeBiblioteca([FromRoute] Guid id,
        [FromBody] PeticionActualizarUserItemDto dto, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.ActualizarItemDeBiblioteca(userId, id, dto, cancellation);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un elemento de la biblioteca del usuario autenticado.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarDeBiblioteca([FromRoute] Guid id, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.EliminarDeBiblioteca(userId, id, cancellation);
        return NoContent();
    }

    /// <summary>
    /// Marcar o desmarcar un elemento como favorito.
    /// </summary>
    [HttpPut("{id:guid}/favorito")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarcarFavorito([FromRoute] Guid id, [FromBody] bool isFavorite,
        CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.MarcarFavorito(userId, id, isFavorite, cancellation);
        return NoContent();
    }

    /// <summary>
    /// Puntuar un elemento de la biblioteca (1 a 5).
    /// </summary>
    [HttpPut("{id:guid}/rating")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Puntuar([FromRoute] Guid id, [FromBody] short rating,
        CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.Puntuar(userId, id, rating, cancellation);
        return NoContent();
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
using System.Security.Claims;
using Asp.Versioning;
using BibliotecaMultimedia.API.Extensions;
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

        PaginacionHeaderHelper.EscribirMetadataPaginacion(Response, resultado.Metadata);

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
    /// Obtener la entrada de biblioteca del usuario autenticado para un ítem del catálogo.
    /// Devuelve 204 si el ítem aún no está en su biblioteca.
    /// </summary>
    [HttpGet("item/{itemId:guid}")]
    [ProducesResponseType(typeof(RespuestaUserItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ObtenerItemDeBibliotecaPorItemId([FromRoute] Guid itemId,
        CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        RespuestaUserItemDto? resultado =
            await _bibliotecaService.ObtenerItemDeBibliotecaPorItemId(userId, itemId, cancellation);

        return resultado is null ? NoContent() : Ok(resultado);
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarcarFavorito([FromRoute] Guid id,
        [FromBody] PeticionMarcarFavoritoDto dto, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.MarcarFavorito(userId, id, dto.IsFavorite, cancellation);
        return NoContent();
    }

    /// <summary>
    /// Puntuar un elemento de la biblioteca (1 a 5).
    /// </summary>
    [HttpPut("{id:guid}/rating")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Puntuar([FromRoute] Guid id,
        [FromBody] PeticionPuntuarDto dto, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.Puntuar(userId, id, dto.Rating, cancellation);
        return NoContent();
    }

    // ===== Préstamos =====

    /// <summary>
    /// Historial de préstamos de un título de la biblioteca.
    /// </summary>
    [HttpGet("{id:guid}/prestamos")]
    [ProducesResponseType(typeof(IEnumerable<RespuestaPrestamoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPrestamos([FromRoute] Guid id, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        IEnumerable<RespuestaPrestamoDto> prestamos =
            await _bibliotecaService.ObtenerPrestamos(userId, id, cancellation);
        return Ok(prestamos);
    }

    /// <summary>
    /// Registrar el préstamo de un título a una persona.
    /// </summary>
    [HttpPost("{id:guid}/prestamos")]
    [ProducesResponseType(typeof(RespuestaPrestamoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AgregarPrestamo([FromRoute] Guid id,
        [FromBody] PeticionCrearPrestamoDto dto, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        RespuestaPrestamoDto resultado = await _bibliotecaService.AgregarPrestamo(userId, id, dto, cancellation);
        return CreatedAtAction(nameof(ObtenerPrestamos), new { id }, resultado);
    }

    /// <summary>
    /// Corregir persona/notas de un préstamo.
    /// </summary>
    [HttpPut("prestamos/{prestamoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarPrestamo([FromRoute] Guid prestamoId,
        [FromBody] PeticionActualizarPrestamoDto dto, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.ActualizarPrestamo(userId, prestamoId, dto, cancellation);
        return NoContent();
    }

    /// <summary>
    /// Registrar la devolución de un préstamo. Sin fecha en el body se usa la fecha actual.
    /// </summary>
    [HttpPut("prestamos/{prestamoId:guid}/devolucion")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegistrarDevolucion([FromRoute] Guid prestamoId,
        CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.RegistrarDevolucion(userId, prestamoId, cancellationToken: cancellation);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un registro de préstamo.
    /// </summary>
    [HttpDelete("prestamos/{prestamoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarPrestamo([FromRoute] Guid prestamoId, CancellationToken cancellation)
    {
        Guid userId = ObtenerUserId();
        await _bibliotecaService.EliminarPrestamo(userId, prestamoId, cancellation);
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
using Asp.Versioning;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMultimedia.API.Extensions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra los ítems (Películas, Juegos, etc.)
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
    }

    /// <summary>
    /// Obtener ítems de forma paginada y con filtros
    /// </summary>
    /// <param name="filtroItem">Filtro para la paginación y búsqueda</param>
    /// <param name="cancellation">Token de cancelación</param>
    /// <returns>Lista paginada de ítems</returns>
    [HttpGet("paginado")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RespuestaItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerItemsPaginado([FromQuery] FiltroItem filtroItem,
        CancellationToken cancellation)
    {
        RespuestaPaginada<RespuestaItemDto> resultado = await _itemService.ObtenerItemsPaginado(filtroItem, cancellation);

        PaginacionHeaderHelper.EscribirMetadataPaginacion(Response, resultado.Metadata);

        return Ok(resultado.Registros);
    }

    /// <summary>
    /// Obtener los ítems destacados (novedades del catálogo) para la vista de descubrimiento
    /// </summary>
    /// <param name="cantidad">Cantidad máxima de ítems a devolver (1-50)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de ítems destacados</returns>
    [HttpGet("destacados")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RespuestaItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerDestacados([FromQuery] int cantidad = 12,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<RespuestaItemDto> destacados = await _itemService.ObtenerDestacados(cantidad, cancellationToken);
        return Ok(destacados);
    }

    /// <summary>
    /// Obtener la distribución de ítems por tipo de medio (agregado en base de datos)
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de tipos de medio con cantidad y porcentaje</returns>
    [HttpGet("distribucion")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<RespuestaDistribucionItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerDistribucionPorTipoMedio(CancellationToken cancellationToken)
    {
        return Ok(await _itemService.ObtenerDistribucionPorTipoMedio(cancellationToken));
    }

    /// <summary>
    /// Obtener todos los ítems registrados
    /// </summary>
    /// <returns>Lista completa de ítems</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RespuestaItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosItems(CancellationToken cancellationToken)
    {
        IEnumerable<RespuestaItemDto> items = await _itemService.ObtenerItems(cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Obtener un ítem específico por su ID
    /// </summary>
    /// <param name="id">ID único del ítem</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Detalle del ítem encontrado</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespuestaItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerItemPorId([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        RespuestaItemDto item = await _itemService.ObtenerItemPorId(id, cancellationToken);
        return Ok(item);
    }

    /// <summary>
    /// Agregar un nuevo ítem a la biblioteca
    /// </summary>
    /// <param name="iTemDto">Información del ítem a crear</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Ítem recién creado</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")] 
    [ProducesResponseType(typeof(RespuestaItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AgregarItem([FromBody] PeticionCrearItemDto iTemDto, CancellationToken cancellationToken)
    {
        // Extraemos el ID del usuario directamente del Token JWT de forma segura
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        Guid currentUserId = Guid.Parse(userIdClaim);

        RespuestaItemDto itemNuevo = await _itemService.AgregarItem(iTemDto, currentUserId, cancellationToken);
        
        return CreatedAtAction(nameof(ObtenerItemPorId), new { id = itemNuevo.Id }, itemNuevo);
    }

    /// <summary>
    /// Actualizar la información de un ítem existente
    /// </summary>
    /// <param name="id">ID del ítem a actualizar</param>
    /// <param name="itemDto">Nueva información</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estado de la operación</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActualizarItem([FromRoute] Guid id, [FromBody] PeticionActualizarItemDto itemDto, CancellationToken cancellationToken)
    {
        await _itemService.ActualizarItem(id, itemDto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un ítem de la biblioteca
    /// </summary>
    /// <param name="id">ID del ítem a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estado de la operación</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarItem([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _itemService.EliminarItem(id, cancellationToken);
        return NoContent();
    }
}
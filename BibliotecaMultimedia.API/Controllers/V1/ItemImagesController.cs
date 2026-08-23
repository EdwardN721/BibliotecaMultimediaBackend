using System.Security.Claims;
using Asp.Versioning;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Imagenes;
using BibliotecaMultimedia.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra las imagenes en blobstorage
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ItemImagesController : ControllerBase
{
    private readonly IItemImageService _itemImageService;

    public ItemImagesController(IItemImageService itemImageService)
    {
        _itemImageService = itemImageService ??  throw new ArgumentNullException(nameof(itemImageService));
    }

    /// <summary>
    /// Sube un fragmento (chunk) de una imagen de un ítem a Azure Blob Storage.
    /// Solo los administradores pueden subir imágenes de contenido.
    /// </summary>
    /// <param name="itemId">Id del ítem al que pertenece la imagen</param>
    /// <param name="chunk">Fragmento binario del archivo</param>
    /// <param name="fileName">Nombre del archivo final</param>
    /// <param name="chunkIndex">Índice del fragmento (0-based)</param>
    /// <param name="totalChunks">Total de fragmentos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estado del procesamiento del fragmento</returns>
    [HttpPost("{itemId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RespuestaUploadChunkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SubirImagenChunk(
        [FromRoute] Guid itemId,
        [FromForm] IFormFile chunk,
        [FromForm] string fileName,
        [FromForm] int chunkIndex,
        [FromForm] int totalChunks,
        CancellationToken cancellationToken
    )
    {
        using Stream fileStream = chunk.OpenReadStream();
        
        RespuestaUploadChunkDto resultado = await _itemImageService.ProcesarChunkAsync(
            itemId,
            fileStream,
            fileName,
            chunk.ContentType,
            chunkIndex,
            totalChunks,
            cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Lista todas las imágenes de un ítem (la principal primero).
    /// </summary>
    [HttpGet("item/{itemId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<RespuestaImagenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObtenerPorItem(Guid itemId, CancellationToken cancellationToken)
    {
        IEnumerable<RespuestaImagenDto> imagenes = await _itemImageService.ObtenerPorItemAsync(itemId, cancellationToken);
        return Ok(imagenes);
    }

    /// <summary>
    /// Elimina una imagen (blob en Azure + registro). Solo administradores.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarImagen(Guid id, CancellationToken cancellationToken)
    {
        await _itemImageService.EliminarImagenAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Marca una imagen como principal del ítem. Solo administradores.
    /// </summary>
    [HttpPut("{id:guid}/principal")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RespuestaImagenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarcarPrincipal(Guid id, CancellationToken cancellationToken)
    {
        RespuestaImagenDto imagen = await _itemImageService.MarcarPrincipalAsync(id, cancellationToken);
        return Ok(imagen);
    }
}
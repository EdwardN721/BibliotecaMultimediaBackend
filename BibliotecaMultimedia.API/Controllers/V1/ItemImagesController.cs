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
}
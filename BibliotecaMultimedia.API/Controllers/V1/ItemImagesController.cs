using BibliotecaMultimedia.Application.DTOs.Respuesta.Imagenes;
using BibliotecaMultimedia.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra las imagenes en blobstorage
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ItemImagesController : ControllerBase
{
    private readonly IItemImageService _itemImageService;

    public ItemImagesController(IItemImageService itemImageService)
    {
        _itemImageService = itemImageService ??  throw new ArgumentNullException(nameof(itemImageService));
    }

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
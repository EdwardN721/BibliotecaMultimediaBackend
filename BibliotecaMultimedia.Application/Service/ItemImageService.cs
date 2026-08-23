using System.Linq.Expressions;
using System.Text;
using BibliotecaMultimedia.Application.DTOs.Peticion.Images;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Imagenes;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BibliotecaMultimedia.Application.Service;

public class ItemImageService : IItemImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ItemImageService> _logger;
    private readonly IBlobStorageService _blobStorageService;

    public ItemImageService(IUnitOfWork unitOfWork, ILogger<ItemImageService> logger, IBlobStorageService blobStorageService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
    }

    public async Task<RespuestaPaginada<RespuestaImagenDto>> ObtenerImagenesPaginados(FiltroImagen filtroImagen, CancellationToken cancellationToken = default)
    {
        Expression<Func<ItemImage, bool>>? filtro = null;

        if (!string.IsNullOrEmpty(filtroImagen.TerminoBusqueda))
        {
            string termino = filtroImagen.TerminoBusqueda.ToLower();
            filtro = i => i.ImageUrl.ToLower().Contains(termino);
        }
        
        (IEnumerable<ItemImage> registros, int total) = await _unitOfWork.CreadoresImagenes.ObtenerPaginadosAsync(
            filter: filtro,
            pageNumber: filtroImagen.PageNumber,
            pageSize: filtroImagen.PageSize,
            includeProperties: null,
            ordenarPor: filtroImagen.OrdenarPor,
            ordenDescendente: filtroImagen.OrdenDescendente,
            cancellationToken: cancellationToken);
        
        int totalPaginas = (int)Math.Ceiling(total / (double)filtroImagen.PageSize);
        
        RespuestaPaginada<RespuestaImagenDto> respuesta = registros 
            .MapToDto()
            .ToRespuestaPaginada(total, totalPaginas, filtroImagen.PageNumber, filtroImagen.PageSize);
        
        _logger.LogInformation("Imagenes paginadas: Página {Page} de {TotalPages} con {Count} registros", 
            respuesta.Metadata.PaginaActual, respuesta.Metadata.TotalPaginas, respuesta.Registros.Count());
        return respuesta;
    }

    public async Task<IEnumerable<RespuestaImagenDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        List<ItemImage> imagenes = (await _unitOfWork.CreadoresImagenes.ObtenerTodosAsync(includeProperties: null, cancellationToken)).ToList();
        
        _logger.LogInformation("Imagenes obtenidas: {Count}", imagenes.Count);
        return imagenes.MapToDto();
    }

    public async Task<RespuestaImagenDto> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ItemImage imagen = await ObtenerItemImage(id, false, cancellationToken);

        return imagen.MapToDto();
    }

    public async Task<IEnumerable<RespuestaImagenDto>> ObtenerPorItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        List<ItemImage> imagenes = (await _unitOfWork.CreadoresImagenes.FindAsync(
            i => i.ItemId == itemId, cancellationToken)).ToList();

        // La imagen principal siempre primero; el resto por fecha de creación
        imagenes = imagenes
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.CreatedAt)
            .ToList();

        _logger.LogInformation("Imagenes obtenidas para el item {ItemId}: {Count}", itemId, imagenes.Count);
        return imagenes.MapToDto();
    }

    public async Task<RespuestaImagenDto> AgregarImagenAsync(PeticionAgregarImagenDto imagenDto, CancellationToken cancellationToken = default)
    {
        ItemImage imagen = imagenDto.MapToEntity();
        
        await _unitOfWork.CreadoresImagenes.AgregarAsync(imagen, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Imagen {Id} agregado", imagen.Id);
        return imagen.MapToDto();
    }

    public async Task ActualizarImagenAsync(Guid id, PeticionActualizarImagenDto imagenDtoDto,
        CancellationToken cancellationToken = default)
    {
        ItemImage imagen = await ObtenerItemImage(id, track: true, cancellationToken);
        
        imagen.UpdateEntity(imagenDtoDto);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Imagen con el Id: {Id} actualizado", imagen.Id);
    }

    public async Task EliminarImagenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ItemImage imagen = await ObtenerItemImage(id, track: true, cancellationToken);

        // Borramos primero el blob; si Azure falla no perdemos el registro en BD
        string? blobName = ExtraerBlobName(imagen.ImageUrl);
        if (blobName is not null)
        {
            try
            {
                await _blobStorageService.EliminarArchivoAsync(blobName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo eliminar el blob {BlobName} de la imagen {Id}", blobName, id);
                throw;
            }
        }
        else
        {
            _logger.LogWarning("La imagen {Id} tiene una URL no reconocida ({Url}); solo se elimina el registro", id, imagen.ImageUrl);
        }

        _unitOfWork.CreadoresImagenes.Eliminar(imagen);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogWarning("Imagen con el Id: {Id} eliminado", imagen.Id);
    }

    public async Task<RespuestaImagenDto> MarcarPrincipalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ItemImage imagen = await ObtenerItemImage(id, track: true, cancellationToken);

        if (!imagen.IsPrimary)
        {
            // Solo una imagen principal por ítem: desmarcamos las demás
            IEnumerable<ItemImage> otrasPrincipales = await _unitOfWork.CreadoresImagenes.FindAsync(
                i => i.ItemId == imagen.ItemId && i.Id != imagen.Id && i.IsPrimary, cancellationToken);

            foreach (ItemImage otra in otrasPrincipales)
            {
                otra.IsPrimary = false;
                _unitOfWork.CreadoresImagenes.Actualizar(otra);
            }

            imagen.IsPrimary = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Imagen {Id} marcada como principal del item {ItemId}", imagen.Id, imagen.ItemId);
        return imagen.MapToDto();
    }

    public async Task<RespuestaUploadChunkDto> ProcesarChunkAsync(Guid itemId, Stream chunkStream, string fileName, string contentType, int chunkIndex,
        int totalChunks, CancellationToken cancellationToken = default)
    {
        Item? itemExiste = await _unitOfWork.Items.GetFirstOrDefaultAsync(i => i.Id == itemId, cancellationToken, disableTracking: true);    
        if (itemExiste == null) 
        {
            throw new NotFoundException($"No se encontró el ítem {itemId}");
        }

        if (chunkIndex < 0 || totalChunks <= 0 || chunkIndex >= totalChunks)
        {
            throw new BusinessRuleException("Los índices de fragmentos no son válidos.");
        }

        string nombreSeguro = SanitizarFileName(fileName);
        
        string blobName = $"items/{itemId}/images/{nombreSeguro}";
        string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(chunkIndex.ToString("d6")));
        
        await _blobStorageService.SubirArchivosChunkAsync(blobName, blockId, chunkStream, cancellationToken);

        if (chunkIndex == totalChunks - 1)
        {
            IEnumerable<string> todosLosBloques = Enumerable.Range(0, totalChunks)
                .Select(i => Convert.ToBase64String(Encoding.UTF8.GetBytes(i.ToString("d6"))));
            
            string urlFinal = await _blobStorageService.ConsolidarChunksAsync(blobName, todosLosBloques, contentType, cancellationToken);
            ItemImage nuevaImagen = new ItemImage
            {
                ItemId = itemId,
                ImageUrl = urlFinal,
            };
            
            await _unitOfWork.CreadoresImagenes.AgregarAsync(nuevaImagen, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Imagen consolidada y guardada para el Item {ItemId}", itemId);
            
            return ImagenMapper.MapUploadChunkSuccessToDto(urlFinal);
        }
        
        return ImagenMapper.MapUploadChunkFailedToDto(chunkIndex, totalChunks);
    }

    #region MetodosPrivados

    /// <summary>
    /// Reconstruye el blobName a partir de la URL pública.
    /// Las URLs tienen la forma .../{contenedor}/items/{itemId}/images/{archivo}
    /// </summary>
    private static string? ExtraerBlobName(string imageUrl)
    {
        int idx = imageUrl.IndexOf("/items/", StringComparison.OrdinalIgnoreCase);
        return idx < 0 ? null : imageUrl[(idx + 1)..];
    }

    private static string SanitizarFileName(string fileName)
    {
        string nombre = Path.GetFileName(fileName.Replace('\\', '/'));
        return string.IsNullOrWhiteSpace(nombre) ? "imagen" : nombre;
    }

    private async Task<ItemImage> ObtenerItemImage(Guid id, bool track = true, CancellationToken cancellationToken = default)
    {
        ItemImage? imagen = await _unitOfWork.CreadoresImagenes.GetFirstOrDefaultAsync(
            predicate: i => i.Id == id,
            cancellationToken: cancellationToken,
            includeProperties: null,
            disableTracking: !track
        );
        
        if (imagen == null)
        {
            _logger.LogWarning("No se encontro la imagen por el Id: {id}", id);
            throw new NotFoundException($"No se encontro la imagen por el Id: {id}");
        }
        return imagen;
    }

    #endregion
}
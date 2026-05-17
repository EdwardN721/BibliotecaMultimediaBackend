using System.Linq.Expressions;
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

    public ItemImageService(IUnitOfWork unitOfWork, ILogger<ItemImageService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        ItemImage imagen = await ObtenerItemImage(id, false, cancellationToken);
        
        imagen.UpdateEntity(imagenDtoDto);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Imagen con el Id: {Id} actualizado", imagen.Id);
    }

    public async Task EliminarImagenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ItemImage imagen = await ObtenerItemImage(id, false, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogWarning("Imagen con el Id: {Id} eliminado", imagen.Id);
    }
    
    #region MetodosPrivados

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
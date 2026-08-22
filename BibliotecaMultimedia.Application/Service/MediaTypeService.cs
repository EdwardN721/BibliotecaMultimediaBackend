using System.Linq.Expressions;
using BibliotecaMultimedia.Application.DTOs.Peticion.MediaType;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.MediaType;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BibliotecaMultimedia.Application.Service;

public class MediaTypeService : IMediaTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MediaTypeService> _logger;

    public MediaTypeService(IUnitOfWork unitOfWork, ILogger<MediaTypeService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RespuestaPaginada<RespuestaMediaTypeDto>> ObtenerMediaTypePaginado(FiltroMediaType filtroMediaType, CancellationToken cancellation = default)
    {
        Expression<Func<MediaType, bool>>? filtro = null;

        if (!string.IsNullOrWhiteSpace(filtroMediaType.TerminoBusqueda))
        {
            string termino = filtroMediaType.TerminoBusqueda.ToLower();
            filtro = m => m.Name.ToLower().Contains(termino);
        }
        
        (IEnumerable<MediaType> registros, int total) = await _unitOfWork.TiposMedia.ObtenerPaginadosAsync(
            filter: filtro,
            pageNumber: filtroMediaType.PageNumber,
            pageSize: filtroMediaType.PageSize,
            ordenarPor: filtroMediaType.OrdenarPor,
            ordenDescendente: filtroMediaType.OrdenDescendente,
            cancellationToken: cancellation);
        
        int totalPaginas = (int)Math.Ceiling(total / (double)filtroMediaType.PageSize);

        RespuestaPaginada<RespuestaMediaTypeDto> respuesta = registros
            .MapToDto()
            .ToRespuestaPaginada(total, totalPaginas, filtroMediaType.PageNumber, filtroMediaType.PageSize);

        _logger.LogInformation("MediaTypes paginadas: Página {Page} de {TotalPages} con {Count} registros", 
            respuesta.Metadata.PaginaActual, respuesta.Metadata.TotalPaginas, respuesta.Registros.Count());
        return respuesta;
    }

    public async Task<IEnumerable<RespuestaMediaTypeDto>> ObtenerMediaTypeTodos(CancellationToken cancellationToken = default)
    {
        List<MediaType> mediaTypes = (await _unitOfWork.TiposMedia.ObtenerTodosAsync(cancellationToken: cancellationToken)).ToList();
        
        _logger.LogInformation("Total de resultados: {Count}", mediaTypes.Count);
        return mediaTypes.MapToDto();
    }

    public async Task<RespuestaMediaTypeDto> ObtenerMediaTypePorId(Guid id, CancellationToken cancellationToken = default)
    {
        MediaType mediaType = await ObtenerMediaType(id, cancellationToken);
        
        return mediaType.MapToDto();
    }

    public async Task<RespuestaMediaTypeDto> AgregarMediaType(PeticionCrearMediaTypeDto dtoEntity, CancellationToken cancellationToken = default)
    {
        MediaType mediaType = dtoEntity.MapToEntity();
        await _unitOfWork.TiposMedia.AgregarAsync(mediaType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Agregado media type Id: {id}", mediaType.Id);
        return mediaType.MapToDto();
    }

    public async Task ActualizarMediaType(Guid id, PeticionActualizarMediaTypeDto dtoEntity,
        CancellationToken cancellationToken = default)
    {
        MediaType mediaType = await ObtenerMediaType(id, cancellationToken);
        
        mediaType.UpdateEntity(dtoEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Actualizado el tipo de medio Id: {id}", mediaType.Id);
    }

    public async Task EliminarMediaType(Guid id, CancellationToken cancellationToken = default)
    {
        MediaType mediaType = await ObtenerMediaType(id, cancellationToken);
        _unitOfWork.TiposMedia.Eliminar(mediaType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogWarning("Eliminado el tipo de media Id: {id}", mediaType.Id);
    }

    #region MetodosPrivados

    private async Task<MediaType> ObtenerMediaType(Guid id, CancellationToken cancellationToken = default)
    {
        MediaType? mediaType = await _unitOfWork.TiposMedia.ObtenerPorIdAsync(id, cancellationToken);
        if (mediaType == null)
        {
            _logger.LogWarning("No se encontro el tipo de medio por el Id {id}", id);
            throw new NotFoundException($"No se encontro el tipo de medio por el Id {id}");
        }
        return mediaType;
    }

    #endregion
}
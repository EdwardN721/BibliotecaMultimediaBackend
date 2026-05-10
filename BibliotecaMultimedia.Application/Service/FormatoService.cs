using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Formatos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

namespace BibliotecaMultimedia.Application.Service;

public class FormatoService : IFormatoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FormatoService> _logger;

    public FormatoService(IUnitOfWork unitOfWork, ILogger<FormatoService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RespuestaPaginada<RespuestaFormatoDto>> ObtenerFormatosPaginados(FiltroFormato filtroFormato, CancellationToken cancellationToken = default)
    {
        Expression<Func<Format, bool>>? filtro = null;

        if (!string.IsNullOrWhiteSpace(filtroFormato.TerminoBusqueda))
        {
            string termino = filtroFormato.TerminoBusqueda.ToLower();
            filtro = f => f.Name.ToLower().Contains(termino);
        }

        (IEnumerable<Format> registros, int total) = await _unitOfWork.Formatos.ObtenerPaginadosAsync(
            filter: filtro,
            pageNumber: filtroFormato.PageNumber,
            pageSize: filtroFormato.PageSize,
            cancellationToken: cancellationToken);
        
        int totalPaginas = (int)Math.Ceiling(total / (double)filtroFormato.PageSize);
        
        RespuestaPaginada<RespuestaFormatoDto> respuesta = registros
            .MapToDto()
            .ToRespuestaPaginada(total, totalPaginas, filtroFormato.PageNumber, filtroFormato.PageSize);
        
        _logger.LogInformation("Formatos paginados: Página {Page} de {TotalPages} con {Count} registros", 
            respuesta.Metadata.PaginaActual, respuesta.Metadata.TotalPaginas, respuesta.Registros.Count());
        return respuesta;
    }

    public async Task<IEnumerable<RespuestaFormatoDto>> ObtenerFormatos(CancellationToken cancellation = default)
    {
        List<Format> formatos = (await _unitOfWork.Formatos.ObtenerTodosAsync(cancellation)).ToList();
        
        _logger.LogInformation("Total de resultados: {Count}", formatos.Count);
        return formatos.MapToDto();
    }

    public async Task<RespuestaFormatoDto> ObtenerFormatoPorId(Guid id, CancellationToken cancellation = default)
    {
        Format formato = await ObtenerFormato(id, cancellation);
        
        _logger.LogInformation("Formato Id: {id} encontrado.", formato.Id);
        return formato.MapToDto();
    }

    public async Task<RespuestaFormatoDto> AgregarFormato(PeticionCrearFormatoDto formatoDto, CancellationToken cancellation = default)
    {
        Format formato = formatoDto.MapToEntity();
        await _unitOfWork.Formatos.AgregarAsync(formato, cancellation);
        await _unitOfWork.SaveChangesAsync(cancellation);
        
        _logger.LogInformation("Agregado Formato: {Nombre}", formato.Name);
        return formato.MapToDto();
    }

    public async Task ActualizarFormato(Guid id, PeticionActualizarFormatoDto formatoDto, CancellationToken cancellation = default)
    {
        Format formato = await ObtenerFormato(id, cancellation);
        
        formato.UpdateEntity(formatoDto);
        await _unitOfWork.SaveChangesAsync(cancellation);
        
        _logger.LogInformation("Actualizado el formato con el Id: {Id}", id);

    }

    public async Task EliminarFormato(Guid id, CancellationToken cancellation = default)
    {
        Format formato = await ObtenerFormato(id, cancellation);
        _unitOfWork.Formatos.Eliminar(formato);
        await _unitOfWork.SaveChangesAsync(cancellation);
        
        _logger.LogWarning("Eliminado el formato con el Id: {Id}", id);

    }
    
    #region MetodosPrivados

    private async Task<Format> ObtenerFormato(Guid id, CancellationToken cancellationToken = default)
    {
        Format? formato = await _unitOfWork.Formatos.ObtenerPorIdAsync(id, cancellationToken);
        if (formato == null)
        {
            _logger.LogWarning("No se encontro el formato por el Id {id}", id);
            throw new NotFoundException($"No se encontro el formato por el Id {id}");
        }
        return formato;
    }

    #endregion
}
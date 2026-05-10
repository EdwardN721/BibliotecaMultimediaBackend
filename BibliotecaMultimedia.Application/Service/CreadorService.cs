using System.Linq.Expressions;
using BibliotecaMultimedia.Application.DTOs.Peticion.Creador;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Creador;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BibliotecaMultimedia.Application.Service;

public class CreadorService : ICreadorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreadorService> _logger;

    public CreadorService(IUnitOfWork unitOfWork, ILogger<CreadorService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RespuestaPaginada<RespuestaCreadorDto>> ObtenerCreadoresPaginado(FiltroCreador filtroCreador, CancellationToken cancellationToken = default)
    {
        Expression<Func<Creator, bool>>? filtro = null;

        if (!string.IsNullOrWhiteSpace(filtroCreador.TerminoBusqueda))
        {
            string termino = filtroCreador.TerminoBusqueda.ToLower();
            filtro = c => c.Name.ToLower().Contains(termino);
        }

        (IEnumerable<Creator> registros, int total) = await _unitOfWork.Creadores.ObtenerPaginadosAsync(
            filter: filtro,
            pageNumber: filtroCreador.PageNumber,
            pageSize: filtroCreador.PageSize,
            cancellationToken: cancellationToken);
        
        int totalPaginas = (int)Math.Ceiling(total / (double)filtroCreador.PageSize);
        
        RespuestaPaginada<RespuestaCreadorDto> respuesta = registros
            .MapToDto()
            .ToRespuestaPaginada(total, totalPaginas, filtroCreador.PageNumber, filtroCreador.PageSize);
        
        _logger.LogInformation("Creadores paginados: Página {Page} de {TotalPages} con {Count} registros", 
            respuesta.Metadata.PaginaActual, respuesta.Metadata.TotalPaginas, respuesta.Registros.Count());
        return respuesta;
    }

    public async Task<IEnumerable<RespuestaCreadorDto>> ObtenerCreadores(CancellationToken cancellation = default)
    {
        List<Creator> creadores = (await _unitOfWork.Creadores.ObtenerTodosAsync(cancellation)).ToList();
        
        _logger.LogInformation("Creadores encontrados: {Count}", creadores.Count);
        return creadores.MapToDto();
    }

    public async Task<RespuestaCreadorDto> ObtenerCreadorPorId(Guid id, CancellationToken cancellation = default)
    {
        Creator creador = await ObtenerCreador(id, cancellation);
        
        _logger.LogInformation("Creador encontrado: {Creador}", creador.Name);
        return creador.MapToDto();
    }

    public async Task<RespuestaCreadorDto> AgregarCreador(PeticionCrearCreadorDto creadorDto, CancellationToken cancellation = default)
    {
        Creator creator = creadorDto.MapToEntity();
        await _unitOfWork.Creadores.AgregarAsync(creator, cancellation);
        await _unitOfWork.SaveChangesAsync(cancellation);
        
        _logger.LogInformation("Creador agregado: {Creador}", creator.Name);
        
        return creator.MapToDto(); 
    }

    public async Task ActualizarCreador(Guid id, PeticionActualizarCreadorDto formato, CancellationToken cancellation = default)
    {
        Creator creador = await ObtenerCreador(id, cancellation);
        
        creador.UpdateEntity(formato);
        
        await _unitOfWork.SaveChangesAsync(cancellation);
        _logger.LogInformation("Creador actualizado: {Creador}", creador.Name);
    }

    public async Task EliminarCreador(Guid id, CancellationToken cancellation = default)
    {
        Creator creador = await ObtenerCreador(id, cancellation);
        _unitOfWork.Creadores.Eliminar(creador);
        await _unitOfWork.SaveChangesAsync(cancellation);
        
        _logger.LogWarning("Creador eliminado: {Creador}", creador.Name);
    }

    #region MetodosPrivados

    private async Task<Creator> ObtenerCreador(Guid id, CancellationToken cancellationToken = default)
    {
        Creator? creador = await _unitOfWork.Creadores.ObtenerPorIdAsync(id, cancellationToken);
        if (creador == null)
        {
            _logger.LogWarning("No se encontro el creador con el Id {id}", id);
            throw new NotFoundException($"No se encontro el creador con el Id {id}");
        }
        return creador;
    }

    #endregion
}
using System.Linq.Expressions;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Peticion.Plataformas;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Plataformas;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Domain.Exceptions;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BibliotecaMultimedia.Application.Service;

public class PlataformaService : IPlataformaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PlataformaService> _logger;

    public PlataformaService(IUnitOfWork unitOfWork, ILogger<PlataformaService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RespuestaPaginada<RespuestaPlataformaDto>> ObtenerPlataformasPaginado(FiltroPlataforma filtroPlataforma, CancellationToken cancellationToken = default)
    {
        Expression<Func<Platform, bool>>? filtro = null;

        if (!string.IsNullOrWhiteSpace(filtroPlataforma.TerminoBusqueda))
        {
            string termino = filtroPlataforma.TerminoBusqueda.ToLower();
            filtro = p => p.Name.ToLower().Contains(termino);
        }

        (IEnumerable<Platform> registros, int total) = await _unitOfWork.Plataformas.ObtenerPaginadosAsync(
            filter: filtro,
            pageNumber: filtroPlataforma.PageNumber,
            pageSize: filtroPlataforma.PageSize,
            cancellationToken: cancellationToken
        );

        int totalPaginas = (int)Math.Ceiling(total / (double)filtroPlataforma.PageSize);
        
        RespuestaPaginada<RespuestaPlataformaDto> respuesta = registros
            .MapToDto()
            .ToRespuestaPaginada(total, totalPaginas, filtroPlataforma.PageNumber, filtroPlataforma.PageSize); 
        
        _logger.LogInformation("Plataformas paginadas: Página {Page} de {TotalPages} con {Count} registros", 
            respuesta.Metadata.PaginaActual, respuesta.Metadata.TotalPaginas, respuesta.Registros.Count());
        return respuesta;
    }

    public async Task<IEnumerable<RespuestaPlataformaDto>> ObtenerPlataformas(CancellationToken cancellation = default)
    {
        List<Platform> plataformas = (await _unitOfWork.Plataformas.ObtenerTodosAsync(cancellationToken: cancellation)).ToList();
        _logger.LogInformation("Elementos encontrados: {Count}", plataformas.Count);
        return plataformas.MapToDto();
    }

    public async Task<RespuestaPlataformaDto> ObtenerPlataformaPorId(Guid id, CancellationToken cancellation = default)
    {
        Platform plataforma = await BuscarPorId(id, cancellation);
        _logger.LogInformation("Plataforma con el Id: {Id} encontrada", plataforma.Id);
        return plataforma.MapToDto();
    }

    public async Task<RespuestaPlataformaDto> AgregarPlataforma(PeticionCrearPlataformaDto plataformaDto, CancellationToken cancellation = default)
    {
        Platform plataform = plataformaDto.MapToEntity();
        await _unitOfWork.Plataformas.AgregarAsync(plataform, cancellation);
        await _unitOfWork.SaveChangesAsync(cancellation);
        _logger.LogInformation("Se agregó la plataforma {Nombre} éxitosamente.", plataform.Name);
        return plataform.MapToDto();
    }

    public async Task ActualizarPlataforma(Guid id, PeticionActualizarPlataformaDto plataformaDto, CancellationToken cancellation = default)
    {
        Platform plataforma = await BuscarPorId(id, cancellation);
        plataforma.UpdateEntity(plataformaDto);
        await _unitOfWork.SaveChangesAsync(cancellation);
        
        _logger.LogInformation("Se actualizó la plataforma con el Id: {Id}", id);
    }

    public async Task EliminarPlataforma(Guid id, CancellationToken cancellation = default)
    {
        Platform plataforma = await BuscarPorId(id, cancellation);
        _unitOfWork.Plataformas.Eliminar(plataforma);
        _logger.LogWarning("Se elimino la plataforma con el Id: {Id}", id);
    }

    #region MetodosPrivados

    private async Task<Platform> BuscarPorId(Guid id, CancellationToken cancellation = default)
    {
        Platform? plataforma = await _unitOfWork.Plataformas.ObtenerPorIdAsync(id, cancellation);
        if (plataforma == null)
        {
            _logger.LogWarning("No se encontró la plataforma con el Id: {Id}", id);
            throw new NotFoundException($"No se encontró la plataforma con el Id: {id}");
        }
        return plataforma;
    }

    #endregion
}
using Microsoft.Extensions.Logging;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Exceptions;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.Catalogos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Catalogos;

namespace BibliotecaMultimedia.Application.Service;

public class GeneroService : IGeneroService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GeneroService> _logger;

    public GeneroService(IUnitOfWork unitOfWork, ILogger<GeneroService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<RespuestaGeneroDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        List<Genre> genero = (await _unitOfWork.Generos.ObtenerTodosAsync(cancellationToken)).ToList();
        
        _logger.LogInformation("Generos encontrados: {Count}", genero.Count);
        return genero.MapToDto();
    }

    public async Task<RespuestaGeneroDto> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Genre genero = await BuscarPorId(id, cancellationToken);
        return genero.MapToDto();
    }

    public async Task<RespuestaGeneroDto> AgregarGeneroAsync(PeticionCrearGeneroDto generoDto, CancellationToken cancellationToken = default)
    {
        Genre genero = generoDto.MapToEntity();
        
        await _unitOfWork.Generos.AgregarAsync(genero, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
         
        _logger.LogInformation("Genero agregado exitosamente con el Id: {Id}", genero.Id);
        return genero.MapToDto();
    }

    public async Task ActualizarGeneroAsync(Guid id, PeticionActualizarGeneroDto generoDto,
        CancellationToken cancellationToken = default)
    {
        Genre genero = await BuscarPorId(id, cancellationToken);
        genero.UpdateEntity(generoDto);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Genero actualizado exitosamente con el Id: {Id}", id);
    }

    public async Task EliminarGeneroAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Genre genero = await BuscarPorId(id, cancellationToken);
        _unitOfWork.Generos.Eliminar(genero);
        
        await _unitOfWork.SaveChangesAsync();
        _logger.LogWarning("Genero con el Id: {Id} eliminado éxistosamente", id);
    }

    #region MetodosPrivados

    private async Task<Genre> BuscarPorId(Guid id, CancellationToken cancellationToken = default)
    {
        Genre? genero = await _unitOfWork.Generos.ObtenerPorIdAsync(id, cancellationToken);

        if (genero is null)
        {
            _logger.LogWarning("Genero con el Id: {Id} no encontrado", id);
            throw new NotFoundException($"Genero con el Id: {id} no encontrado");
        }
        return genero;
    }

    #endregion
}
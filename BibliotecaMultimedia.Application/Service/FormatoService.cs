using BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Formatos;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using Microsoft.Extensions.Logging;

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
            throw new KeyNotFoundException($"No se encontro el formato por el Id {id}");
        }
        return formato;
    }

    #endregion
}
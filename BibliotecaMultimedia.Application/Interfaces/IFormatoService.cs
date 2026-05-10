using BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Formatos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IFormatoService
{
    Task<RespuestaPaginada<RespuestaFormatoDto>> ObtenerFormatosPaginados(FiltroFormato filtroFormato, CancellationToken cancellationToken = default);
    Task<IEnumerable<RespuestaFormatoDto>> ObtenerFormatos(CancellationToken cancellation = default);
    Task<RespuestaFormatoDto> ObtenerFormatoPorId(Guid id, CancellationToken cancellation = default);
    Task<RespuestaFormatoDto> AgregarFormato(PeticionCrearFormatoDto formatoDto, CancellationToken cancellation = default);
    Task ActualizarFormato(Guid id, PeticionActualizarFormatoDto formato, CancellationToken cancellation = default);
    Task EliminarFormato(Guid id, CancellationToken cancellation = default);
}
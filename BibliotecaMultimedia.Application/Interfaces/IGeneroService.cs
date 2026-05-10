using BibliotecaMultimedia.Application.DTOs.Peticion.Catalogos;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Catalogos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IGeneroService
{
    Task<RespuestaPaginada<RespuestaGeneroDto>> ObtenerGenerosPaginados(FiltroGenero filtroGenero, CancellationToken cancellationToken = default);
    Task<IEnumerable<RespuestaGeneroDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<RespuestaGeneroDto> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RespuestaGeneroDto> AgregarGeneroAsync(PeticionCrearGeneroDto generoDto, CancellationToken cancellationToken = default);
    Task ActualizarGeneroAsync(Guid id, PeticionActualizarGeneroDto generoDto, CancellationToken cancellationToken = default);
    Task EliminarGeneroAsync(Guid id, CancellationToken cancellationToken = default);
}
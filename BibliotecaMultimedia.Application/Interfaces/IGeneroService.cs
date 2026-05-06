using BibliotecaMultimedia.Application.DTOs.Peticion.Catalogos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Catalogos;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IGeneroService
{
    Task<IEnumerable<RespuestaGeneroDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<RespuestaGeneroDto> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RespuestaGeneroDto> AgregarGeneroAsync(PeticionCrearGeneroDto generoDto, CancellationToken cancellationToken = default);
    Task ActualizarGeneroAsync(Guid id, PeticionActualizarGeneroDto generoDto, CancellationToken cancellationToken = default);
    Task EliminarGeneroAsync(Guid id, CancellationToken cancellationToken = default);
}
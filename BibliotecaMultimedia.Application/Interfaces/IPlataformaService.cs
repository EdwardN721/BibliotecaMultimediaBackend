using BibliotecaMultimedia.Application.DTOs.Peticion.Plataformas;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Plataformas;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IPlataformaService
{
    Task<IEnumerable<RespuestaPlataformaDto>> ObtenerPlataformas(CancellationToken cancellation = default);
    Task<RespuestaPlataformaDto> ObtenerPlataformaPorId(Guid id, CancellationToken cancellation = default);
    Task<RespuestaPlataformaDto> AgregarPlataforma(PeticionCrearPlataformaDto plataforma, CancellationToken cancellation = default);
    Task ActualizarPlataforma(Guid id, PeticionActualizarPlataformaDto plataforma, CancellationToken cancellation = default);
    Task EliminarPlataforma(Guid id, CancellationToken cancellation = default);
}
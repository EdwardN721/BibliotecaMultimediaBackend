using BibliotecaMultimedia.Application.DTOs.Peticion.Creador;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Creador;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface ICreadorService
{
    Task<RespuestaPaginada<RespuestaCreadorDto>> ObtenerCreadoresPaginado(FiltroCreador filtroCreador, CancellationToken cancellationToken = default);
    Task<IEnumerable<RespuestaCreadorDto>> ObtenerCreadores(CancellationToken cancellation = default);
    Task<RespuestaCreadorDto> ObtenerCreadorPorId(Guid id, CancellationToken cancellation = default);
    Task<RespuestaCreadorDto> AgregarCreador(PeticionCrearCreadorDto creadorDto, CancellationToken cancellation = default);
    Task ActualizarCreador(Guid id, PeticionActualizarCreadorDto creadorDto, CancellationToken cancellation = default);
    Task EliminarCreador(Guid id, CancellationToken cancellation = default);
}
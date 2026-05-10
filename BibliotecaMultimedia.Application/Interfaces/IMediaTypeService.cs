using BibliotecaMultimedia.Application.DTOs.Peticion.MediaType;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.MediaType;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IMediaTypeService
{
    Task<RespuestaPaginada<RespuestaMediaTypeDto>> ObtenerMediaTypePaginado(FiltroMediaType filtroMediaType, CancellationToken cancellation = default);
    Task<IEnumerable<RespuestaMediaTypeDto>> ObtenerMediaTypeTodos(CancellationToken cancellationToken = default);
    Task<RespuestaMediaTypeDto> ObtenerMediaTypePorId(Guid id, CancellationToken cancellationToken = default);
    Task<RespuestaMediaTypeDto> AgregarMediaType(PeticionCrearMediaTypeDto dtoEntity, CancellationToken cancellationToken = default);
    Task ActualizarMediaType(Guid id, PeticionActualizarMediaTypeDto dtoEntity, CancellationToken cancellationToken = default);
    Task EliminarMediaType(Guid id, CancellationToken cancellationToken = default);
}
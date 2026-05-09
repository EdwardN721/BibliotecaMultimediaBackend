using BibliotecaMultimedia.Application.DTOs.Peticion.MediaType;
using BibliotecaMultimedia.Application.DTOs.Respuesta.MediaType;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IMediaTypeService
{
    Task<IEnumerable<RespuestaMediaTypeDto>> ObtenerMediaTypeTodos(CancellationToken cancellationToken = default);
    Task<RespuestaMediaTypeDto> ObtenerMediaTypePorId(Guid id, CancellationToken cancellationToken = default);
    Task<RespuestaMediaTypeDto> AgregarMediaType(PeticionCrearMediaTypeDto dtoEntity, CancellationToken cancellationToken = default);
    Task ActualizarMediaType(Guid id, PeticionActualizarMediaTypeDto dtoEntity, CancellationToken cancellationToken = default);
    Task EliminarMediaType(Guid id, CancellationToken cancellationToken = default);
}
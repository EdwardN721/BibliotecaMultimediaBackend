using BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Formatos;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IFormatoService
{
    Task<IEnumerable<RespuestaFormatoDto>> ObtenerFormatos(CancellationToken cancellation = default);
    Task<RespuestaFormatoDto> ObtenerFormatoPorId(Guid id, CancellationToken cancellation = default);
    Task<RespuestaFormatoDto> AgregarFormato(PeticionCrearFormatoDto formatoDto, CancellationToken cancellation = default);
    Task ActualizarFormato(Guid id, PeticionActualizarFormatoDto formato, CancellationToken cancellation = default);
    Task EliminarFormato(Guid id, CancellationToken cancellation = default);
}
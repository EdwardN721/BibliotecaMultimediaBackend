using BibliotecaMultimedia.Application.DTOs.Peticion.Images;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Imagenes;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IItemImageService
{
    Task<RespuestaPaginada<RespuestaImagenDto>> ObtenerImagenesPaginados(FiltroImagen filtroImagen, CancellationToken cancellationToken = default);
    Task<IEnumerable<RespuestaImagenDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<RespuestaImagenDto> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RespuestaImagenDto> AgregarImagenAsync(PeticionAgregarImagenDto imagenDto, CancellationToken cancellationToken = default);
    Task ActualizarImagenAsync(Guid id, PeticionActualizarImagenDto imagenDtoDto, CancellationToken cancellationToken = default);
    Task EliminarImagenAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RespuestaUploadChunkDto> ProcesarChunkAsync(Guid itemId, Stream chunkStream, string fileName,
        string contentType, int chunkIndex, int totalChunks, CancellationToken cancellationToken = default);
}
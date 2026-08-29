using BibliotecaMultimedia.Application.DTOs.Respuesta.Imagenes;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IItemImageService
{
    Task<IEnumerable<RespuestaImagenDto>> ObtenerPorItemAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task EliminarImagenAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RespuestaImagenDto> MarcarPrincipalAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RespuestaUploadChunkDto> ProcesarChunkAsync(Guid itemId, Stream chunkStream, string fileName,
        string contentType, int chunkIndex, int totalChunks, CancellationToken cancellationToken = default);
}
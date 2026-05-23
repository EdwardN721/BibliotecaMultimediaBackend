namespace BibliotecaMultimedia.Application.Interfaces;

public interface IBlobStorageService
{
    Task SubirArchivosChunkAsync(string blobName, string base64BlockId, Stream chunkStream, 
        CancellationToken cancellationToken = default);

    Task<string> ConsolidarChunksAsync(string blobName, IEnumerable<string> blockIds, string contentType, 
        CancellationToken cancellationToken = default);
}
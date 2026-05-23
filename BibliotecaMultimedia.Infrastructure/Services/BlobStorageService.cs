using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using BibliotecaMultimedia.Application.Interfaces;

namespace BibliotecaMultimedia.Infrastructure.Services;

/// <summary>
/// Servicio de infraestructura encargado de gestionar la subida y ensamblaje 
/// de archivos grandes en Azure Blob Storage mediante fragmentos (chunks).
/// </summary>
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "biblioteca-multimedia";
    
    public BlobStorageService(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    /// <summary>
    /// Sube un fragmento específico de un archivo a la memoria temporal de Azure (Block Blob).
    /// </summary>
    /// <remarks>
    /// El bloque subido queda en estado "Uncommitted" (invisible para el usuario) 
    /// hasta que se llame al método de consolidación.
    /// </remarks>
    /// <param name="blobName">La ruta virtual y nombre del archivo final en Azure).</param>
    /// <param name="base64BlockId">Identificador único del fragmento. Azure exige que tenga longitud fija y esté en Base64.</param>
    /// <param name="chunkStream">El flujo de datos (bytes) que contiene la porción de la imagen.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    public async Task SubirArchivosChunkAsync(string blobName, string base64BlockId, Stream chunkStream, 
        CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
        
        BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(blobName);
        
        await blockBlobClient.StageBlockAsync(base64BlockId, chunkStream, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Toma todos los fragmentos previamente subidos a la memoria temporal y los une 
    /// para formar el archivo público y final.
    /// </summary>
    /// <param name="blobName">La ruta virtual y nombre del archivo final en Azure.</param>
    /// <param name="blockIds">Lista ordenada con los IDs en Base64 de todos los bloques a ensamblar.</param>
    /// <param name="contentType">El tipo MIME del archivo.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>La URL pública y absoluta de la imagen recién ensamblada.</returns>
    public async Task<string> ConsolidarChunksAsync(string blobName, IEnumerable<string> blockIds, string contentType, 
        CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(blobName);

        BlobHttpHeaders headers = new BlobHttpHeaders { ContentType = contentType };

        await blockBlobClient.CommitBlockListAsync(blockIds, new CommitBlockListOptions
            { HttpHeaders = headers }, cancellationToken: cancellationToken);
        
        return blockBlobClient.Uri.ToString();
    }
}
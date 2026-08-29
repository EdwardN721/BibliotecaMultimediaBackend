using System.Text.Json;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using Microsoft.AspNetCore.Http;

namespace BibliotecaMultimedia.API.Extensions;

/// <summary>
/// Helper para escribir la metadata de paginación de forma consistente
/// en el header X-Pagination (expuesta a CORS para el frontend).
/// </summary>
public static class PaginacionHeaderHelper
{
    public static void EscribirMetadataPaginacion(HttpResponse response, PaginacionMetadata metadata)
    {
        string metadataJson = JsonSerializer.Serialize(metadata);
        response.Headers.Append("Access-Control-Expose-Headers", "X-Pagination");
        response.Headers.Append("X-Pagination", metadataJson);
    }
}

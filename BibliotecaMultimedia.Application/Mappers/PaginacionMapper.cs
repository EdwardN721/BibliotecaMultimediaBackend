using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;

namespace BibliotecaMultimedia.Application.Mappers;

public static class PaginacionMapper
{
    public static RespuestaPaginada<T> ToRespuestaPaginada<T>(
        this IEnumerable<T> registros,
        int totalRegistros,
        int totalPaginas,
        int pageNumber,
        int pageSize)
    {
        return new RespuestaPaginada<T>
        {
            Registros = registros,
            Metadata  = new PaginacionMetadata
            {
                TotalRegistros = totalRegistros,
                PaginaActual = pageNumber,
                RegistrosPorPagina = pageSize,
                TotalPaginas = totalPaginas,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPaginas
            }
        };
    }
        
        
}
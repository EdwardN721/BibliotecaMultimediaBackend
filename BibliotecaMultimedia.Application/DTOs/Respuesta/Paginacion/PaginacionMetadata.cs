namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;

public class PaginacionMetadata
{
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public int RegistrosPorPagina { get; set; }
    public int TotalRegistros { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
namespace BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

public class FiltroGenero : PeticionPaginacion
{
    public string? TerminoBusqueda { get; set; } 
    public string? OrdenarPor { get; set; }
    public bool OrdenDescendente { get; set; } = false;
}
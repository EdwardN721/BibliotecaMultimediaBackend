namespace BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

public class FiltroMediaType : PeticionPaginacion
{
    public string? TerminoBusqueda { get; set; } 
    public string? OrdenarPor { get; set; }
    public bool OrdenDescendente { get; set; } = false;
}
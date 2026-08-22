namespace BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

using BibliotecaMultimedia.Domain.Enums;

public class FiltroBiblioteca : PeticionPaginacion
{
    public string? TerminoBusqueda { get; set; }
    public ConsumptionStatus? Status { get; set; }
    public bool? IsFavorite { get; set; }
    public string? OrdenarPor { get; set; }
    public bool OrdenDescendente { get; set; } = false;
}
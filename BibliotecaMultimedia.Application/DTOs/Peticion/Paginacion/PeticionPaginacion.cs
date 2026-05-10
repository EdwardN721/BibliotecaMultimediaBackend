namespace BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion;

public class PeticionPaginacion
{
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > 60) ? 60 : value;
    }
}
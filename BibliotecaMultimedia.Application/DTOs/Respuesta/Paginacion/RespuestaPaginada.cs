namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;

public class RespuestaPaginada<T>
{
    public IEnumerable<T> Registros { get; set; } = new List<T>();
    public PaginacionMetadata Metadata { get; set; } = null!;
}


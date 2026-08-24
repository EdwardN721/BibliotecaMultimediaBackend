namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;

public record RespuestaBibliotecaStatsDto
{
    public int TotalItems { get; init; }
    public int Pendientes { get; init; }
    public int EnProgreso { get; init; }
    public int Completados { get; init; }
    public int Abandonados { get; init; }
    public int Deseados { get; init; }
    public int Favoritos { get; init; }
    public double RatingPromedio { get; init; }

    /// <summary>Préstamos sin devolución registrada.</summary>
    public int PrestadosActivos { get; init; }

    /// <summary>Títulos en biblioteca agrupados por tipo de medio (catálogo).</summary>
    public List<RespuestaConteoCatalogoDto> PorMediaType { get; init; } = new();
};

/// <summary>Cantidad de títulos que el usuario tiene en su biblioteca de un tipo de medio.</summary>
public record RespuestaConteoCatalogoDto
{
    public Guid MediaTypeId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public int Cantidad { get; init; }
};

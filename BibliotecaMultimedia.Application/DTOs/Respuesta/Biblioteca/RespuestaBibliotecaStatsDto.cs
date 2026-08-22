namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;

public record RespuestaBibliotecaStatsDto
{
    public int TotalItems { get; init; }
    public int Pendientes { get; init; }
    public int EnProgreso { get; init; }
    public int Completados { get; init; }
    public int Abandonados { get; init; }
    public int Favoritos { get; init; }
    public double RatingPromedio { get; init; }
};

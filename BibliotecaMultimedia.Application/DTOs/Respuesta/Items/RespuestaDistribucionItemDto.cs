namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Items;

/// <summary>
/// Distribución de ítems del catálogo por tipo de medio (para dashboards).
/// </summary>
public record RespuestaDistribucionItemDto
{
    public string Nombre { get; init; } = string.Empty;
    public int Cantidad { get; init; }
    public double Porcentaje { get; init; }
};

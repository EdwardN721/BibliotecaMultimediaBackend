namespace BibliotecaMultimedia.Domain.Enums;

public enum ConsumptionStatus
{
    Pendiente,
    EnProgreso,
    Completado,
    Abandonado,

    /// <summary>
    /// Título que el usuario quiere conseguir algún día (lista de deseos).
    /// Agregado al FINAL del enum: la columna se guarda como string, así que
    /// reordenar los valores existentes rompería los datos ya persistidos.
    /// </summary>
    Deseado
}

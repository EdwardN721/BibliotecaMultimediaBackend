namespace BibliotecaMultimedia.Domain.Models;

public abstract class BaseEntity
{
    // Sin inicializador: el Id lo genera la BD (gen_random_uuid) y EF lo trae
    // vía RETURNING tras el INSERT. Inicializarlo aquí hacía que EF confundiera
    // entidades nuevas con existentes al descubrirlas por navegación.
    public Guid Id { get; protected set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    
    public bool IsDeleted => DeletedAt.HasValue;
}
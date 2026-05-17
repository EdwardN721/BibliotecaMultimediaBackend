namespace BibliotecaMultimedia.Application.DTOs.Peticion.Images;

public record PeticionActualizarImagenDto
{
    public Guid ItemId { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public bool IsPrimary { get; init; } = false;
};
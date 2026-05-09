namespace BibliotecaMultimedia.Application.DTOs.Peticion.Plataformas;

public record PeticionCrearPlataformaDto
{
    public string Nombre { get; init; } = string.Empty;
};
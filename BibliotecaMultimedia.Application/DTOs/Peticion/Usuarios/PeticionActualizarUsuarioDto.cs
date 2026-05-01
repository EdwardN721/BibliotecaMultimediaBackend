namespace BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;

public record PeticionActualizarUsuarioDto
{
    public string? Nombre { get; init; }
    public string? PrimerApellido { get; init; }
    public string? SegundoApellido { get; init; }
    public string? PhoneNumber { get; init; }
};
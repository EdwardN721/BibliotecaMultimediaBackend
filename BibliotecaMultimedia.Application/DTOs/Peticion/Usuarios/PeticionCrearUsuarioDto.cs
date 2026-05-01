namespace BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;

public record PeticionCrearUsuarioDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string PrimerApellido { get; init; } = string.Empty;
    public string? SegundoApellido { get; init; }
    public string? PhoneNumber { get; init; }
};
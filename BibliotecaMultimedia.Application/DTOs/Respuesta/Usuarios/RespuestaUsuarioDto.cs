namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Usuarios;

public record RespuestaUsuarioDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string PrimerApellido { get; init; } = string.Empty;
    public string? SegundoApellido { get; init; }
    public string? PhoneNumber { get; init; }
    
    public string NombreCompleto => string.IsNullOrWhiteSpace(SegundoApellido) 
        ? $"{Nombre} {PrimerApellido}" 
        : $"{Nombre} {PrimerApellido} {SegundoApellido}";
};
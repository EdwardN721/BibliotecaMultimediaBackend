namespace BibliotecaMultimedia.Application.DTOs.Respuesta.Usuarios;

public record AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public RespuestaUsuarioDto User { get; set; } = null!;
};
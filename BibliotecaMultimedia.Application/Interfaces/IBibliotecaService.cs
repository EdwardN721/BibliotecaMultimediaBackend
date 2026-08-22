using BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
namespace BibliotecaMultimedia.Application.Interfaces;

public interface IBibliotecaService
{
    Task<RespuestaPaginada<RespuestaUserItemDto>> ObtenerBibliotecaPaginado(Guid userId, FiltroBiblioteca filtro, CancellationToken cancellationToken = default);
    Task<RespuestaBibliotecaStatsDto> ObtenerStats(Guid userId, CancellationToken cancellationToken = default);
    Task<RespuestaUserItemDto> ObtenerItemDeBiblioteca(Guid userId, Guid userItemId, CancellationToken cancellationToken = default);
    Task<RespuestaUserItemDto> AgregarABiblioteca(Guid userId, PeticionAgregarABibliotecaDto dto, CancellationToken cancellationToken = default);
    Task ActualizarItemDeBiblioteca(Guid userId, Guid userItemId, PeticionActualizarUserItemDto dto, CancellationToken cancellationToken = default);
    Task EliminarDeBiblioteca(Guid userId, Guid userItemId, CancellationToken cancellationToken = default);
    Task MarcarFavorito(Guid userId, Guid userItemId, bool isFavorite, CancellationToken cancellationToken = default);
    Task Puntuar(Guid userId, Guid userItemId, short rating, CancellationToken cancellationToken = default);
}
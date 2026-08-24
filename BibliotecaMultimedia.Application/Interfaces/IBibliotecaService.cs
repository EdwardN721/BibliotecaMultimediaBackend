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

    /// <summary>
    /// Devuelve la entrada de biblioteca del usuario autenticado para un ítem del catálogo,
    /// o null si aún no lo tiene agregado.
    /// </summary>
    Task<RespuestaUserItemDto?> ObtenerItemDeBibliotecaPorItemId(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
    Task<RespuestaUserItemDto> AgregarABiblioteca(Guid userId, PeticionAgregarABibliotecaDto dto, CancellationToken cancellationToken = default);
    Task ActualizarItemDeBiblioteca(Guid userId, Guid userItemId, PeticionActualizarUserItemDto dto, CancellationToken cancellationToken = default);
    Task EliminarDeBiblioteca(Guid userId, Guid userItemId, CancellationToken cancellationToken = default);
    Task MarcarFavorito(Guid userId, Guid userItemId, bool isFavorite, CancellationToken cancellationToken = default);
    Task Puntuar(Guid userId, Guid userItemId, short rating, CancellationToken cancellationToken = default);

    // ===== Préstamos =====

    /// <summary>Historial de préstamos de un título de la biblioteca (más recientes primero).</summary>
    Task<IEnumerable<RespuestaPrestamoDto>> ObtenerPrestamos(Guid userId, Guid userItemId, CancellationToken cancellationToken = default);

    /// <summary>Todos los préstamos activos del usuario (títulos que aún no le devuelven).</summary>
    Task<IEnumerable<RespuestaPrestamoDto>> ObtenerPrestamosActivos(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Registra el préstamo de un título a una persona.</summary>
    Task<RespuestaPrestamoDto> AgregarPrestamo(Guid userId, Guid userItemId, PeticionCrearPrestamoDto dto, CancellationToken cancellationToken = default);

    /// <summary>Corrige persona/notas de un préstamo.</summary>
    Task ActualizarPrestamo(Guid userId, Guid prestamoId, PeticionActualizarPrestamoDto dto, CancellationToken cancellationToken = default);

    /// <summary>Registra la devolución del préstamo. Sin fecha explícita se usa ahora.</summary>
    Task RegistrarDevolucion(Guid userId, Guid prestamoId, DateTimeOffset? fechaDevolucion = null, CancellationToken cancellationToken = default);

    /// <summary>Elimina un registro de préstamo.</summary>
    Task EliminarPrestamo(Guid userId, Guid prestamoId, CancellationToken cancellationToken = default);
}
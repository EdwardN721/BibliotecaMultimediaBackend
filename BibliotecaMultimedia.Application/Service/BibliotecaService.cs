using System.Linq.Expressions;
using BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Common;
using BibliotecaMultimedia.Application.Extensions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BibliotecaMultimedia.Application.Service;

public class BibliotecaService : IBibliotecaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BibliotecaService> _logger;

    public BibliotecaService(IUnitOfWork unitOfWork, ILogger<BibliotecaService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RespuestaPaginada<RespuestaUserItemDto>> ObtenerBibliotecaPaginado(
        Guid userId, FiltroBiblioteca filtro, CancellationToken cancellationToken = default)
    {
        Expression<Func<UserItem, bool>>? filtroExpresion = null;

        if (filtro.Status.HasValue)
        {
            filtroExpresion = u => u.Status == filtro.Status.Value;
        }

        if (filtro.IsFavorite.HasValue)
        {
            Expression<Func<UserItem, bool>> filtroFavorito = u => u.IsFavorite == filtro.IsFavorite.Value;
            filtroExpresion = filtroExpresion == null ? filtroFavorito : filtroExpresion.And(filtroFavorito);
        }

        if (!string.IsNullOrWhiteSpace(filtro.TerminoBusqueda))
        {
            string termino = filtro.TerminoBusqueda.ToLower();
            Expression<Func<UserItem, bool>> filtroTermino =
                u => u.Item != null && u.Item.Title.ToLower().Contains(termino);
            filtroExpresion = filtroExpresion == null ? filtroTermino : filtroExpresion.And(filtroTermino);
        }

        // Ownership: SIEMPRE se filtra por el usuario autenticado
        Expression<Func<UserItem, bool>> filtroUsuario = u => u.UserId == userId;
        filtroExpresion = filtroExpresion == null ? filtroUsuario : filtroExpresion.And(filtroUsuario);

        (IEnumerable<UserItem> registros, int total) = await _unitOfWork.UserItems.ObtenerPaginadosAsync(
            filter: filtroExpresion,
            pageNumber: filtro.PageNumber,
            pageSize: filtro.PageSize,
            includeProperties: ItemIncludes.DesdeUserItem,
            ordenarPor: filtro.OrdenarPor,
            ordenDescendente: filtro.OrdenDescendente,
            cancellationToken: cancellationToken);

        int totalPaginas = (int)Math.Ceiling(total / (double)filtro.PageSize);

        RespuestaPaginada<RespuestaUserItemDto> respuesta = registros
            .MapToDto()
            .ToRespuestaPaginada(total, totalPaginas, filtro.PageNumber, filtro.PageSize);

        _logger.LogInformation("Biblioteca del usuario {UserId}: Página {Page} de {TotalPages} con {Count} registros",
            userId, respuesta.Metadata.PaginaActual, respuesta.Metadata.TotalPaginas, respuesta.Registros.Count());
        return respuesta;
    }

    public async Task<RespuestaUserItemDto> ObtenerItemDeBiblioteca(Guid userId, Guid userItemId, CancellationToken cancellationToken = default)
    {
        UserItem userItem = await ObtenerUserItem(userId, userItemId, cancellationToken);
        return userItem.MapToDto();
    }

    public async Task<RespuestaUserItemDto?> ObtenerItemDeBibliotecaPorItemId(Guid userId, Guid itemId,
        CancellationToken cancellationToken = default)
    {
        UserItem? userItem = await _unitOfWork.UserItems.GetFirstOrDefaultAsync(
            predicate: u => u.UserId == userId && u.ItemId == itemId,
            cancellationToken: cancellationToken,
            includeProperties: ItemIncludes.DesdeUserItem,
            disableTracking: true);

        return userItem?.MapToDto();
    }

    public async Task<RespuestaBibliotecaStatsDto> ObtenerStats(Guid userId, CancellationToken cancellationToken = default)
    {
        // Cargamos los préstamos del usuario junto con sus UserItem para evitar
        // una segunda consulta agregada sobre toda la tabla de préstamos.
        List<UserItem> items = (await _unitOfWork.UserItems.FindAsync(
            u => u.UserId == userId,
            cancellationToken,
            u => u.Item!.MediaType,
            u => u.Prestamos!)).ToList();

        var ratings = items.Where(i => i.PersonalRating.HasValue).Select(i => (double)i.PersonalRating!.Value).ToList();

        int prestadosActivos = items
            .SelectMany(u => u.Prestamos ?? new List<Prestamo>())
            .Count(p => p.FechaDevolucion == null);

        RespuestaBibliotecaStatsDto stats = new RespuestaBibliotecaStatsDto
        {
            TotalItems = items.Count,
            Pendientes = items.Count(i => i.Status == ConsumptionStatus.Pendiente),
            EnProgreso = items.Count(i => i.Status == ConsumptionStatus.EnProgreso),
            Completados = items.Count(i => i.Status == ConsumptionStatus.Completado),
            Abandonados = items.Count(i => i.Status == ConsumptionStatus.Abandonado),
            Deseados = items.Count(i => i.Status == ConsumptionStatus.Deseado),
            Favoritos = items.Count(i => i.IsFavorite),
            RatingPromedio = ratings.Count == 0 ? 0 : Math.Round(ratings.Average() * 10) / 10,
            PrestadosActivos = prestadosActivos,
            PorMediaType = items
                .Where(i => i.Item?.MediaType != null)
                .GroupBy(i => new { i.Item!.MediaTypeId, i.Item.MediaType.Name })
                .Select(g => new RespuestaConteoCatalogoDto
                {
                    MediaTypeId = g.Key.MediaTypeId,
                    Nombre = g.Key.Name,
                    Cantidad = g.Count(),
                })
                .OrderByDescending(c => c.Cantidad)
                .ThenBy(c => c.Nombre)
                .ToList(),
        };

        _logger.LogInformation("Stats de biblioteca del usuario {UserId}: {Total} items", userId, stats.TotalItems);
        return stats;
    }

    public async Task<RespuestaUserItemDto> AgregarABiblioteca(Guid userId, PeticionAgregarABibliotecaDto dto,
        CancellationToken cancellationToken = default)
    {
        Item? item = await _unitOfWork.Items.GetFirstOrDefaultAsync(
            predicate: i => i.Id == dto.ItemId,
            cancellationToken: cancellationToken,
            disableTracking: true);
        if (item is null)
        {
            _logger.LogWarning("No se encontró el item {ItemId} al agregar a la biblioteca", dto.ItemId);
            throw new NotFoundException($"No se encontró el ítem {dto.ItemId}");
        }

        bool yaExiste = await _unitOfWork.UserItems.GetFirstOrDefaultAsync(
            predicate: u => u.UserId == userId && u.ItemId == dto.ItemId,
            cancellationToken: cancellationToken,
            disableTracking: true) is not null;
        if (yaExiste)
        {
            _logger.LogWarning("El usuario {UserId} ya tiene el item {ItemId} en su biblioteca", userId, dto.ItemId);
            throw new BusinessRuleException("El ítem ya se encuentra en tu biblioteca.");
        }

        UserItem userItem = dto.MapToEntity(userId);
        await ValidarCopiaPropiaAsync(dto.OwnedFormatIds, dto.OwnedPlatformIds, cancellationToken);
        foreach (Guid formatId in dto.OwnedFormatIds.Distinct())
        {
            userItem.UserItemFormats!.Add(new UserItemFormat { FormatId = formatId, UserItemId = Guid.Empty });
        }
        foreach (Guid platformId in dto.OwnedPlatformIds.Distinct())
        {
            userItem.UserItemPlatforms!.Add(new UserItemPlatform { PlatformId = platformId, UserItemId = Guid.Empty });
        }

        await _unitOfWork.UserItems.AgregarAsync(userItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item {ItemId} agregado a la biblioteca del usuario {UserId}", dto.ItemId, userId);

        // Recargamos con los navegaciones incluidas para devolver el DTO completo
        UserItem guardado = await ObtenerUserItem(userId, userItem.Id, cancellationToken);
        return guardado.MapToDto();
    }

    public async Task ActualizarItemDeBiblioteca(Guid userId, Guid userItemId, PeticionActualizarUserItemDto dto,
        CancellationToken cancellationToken = default)
    {
        UserItem userItem = await ObtenerUserItem(userId, userItemId, cancellationToken, track: true);
        userItem.UpdateEntity(dto);

        if (dto.OwnedFormatIds is not null || dto.OwnedPlatformIds is not null)
        {
            await SincronizarCopiaPropiaAsync(userItem, dto, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("UserItem {UserItemId} actualizado por el usuario {UserId}", userItemId, userId);
    }

    public async Task EliminarDeBiblioteca(Guid userId, Guid userItemId, CancellationToken cancellationToken = default)
    {
        UserItem userItem = await ObtenerUserItem(userId, userItemId, cancellationToken);
        _unitOfWork.UserItems.Eliminar(userItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogWarning("UserItem {UserItemId} eliminado de la biblioteca del usuario {UserId}", userItemId, userId);
    }

    public async Task MarcarFavorito(Guid userId, Guid userItemId, bool isFavorite, CancellationToken cancellationToken = default)
    {
        UserItem userItem = await ObtenerUserItem(userId, userItemId, cancellationToken, track: true);
        userItem.IsFavorite = isFavorite;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("UserItem {UserItemId} marcado como favorito={IsFavorite} por {UserId}", userItemId, isFavorite, userId);
    }

    public async Task Puntuar(Guid userId, Guid userItemId, short rating, CancellationToken cancellationToken = default)
    {
        if (rating is < 1 or > 5)
        {
            throw new BusinessRuleException("La calificación debe estar entre 1 y 5.");
        }

        UserItem userItem = await ObtenerUserItem(userId, userItemId, cancellationToken, track: true);
        userItem.PersonalRating = rating;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("UserItem {UserItemId} puntuado con {Rating} por {UserId}", userItemId, rating, userId);
    }

    #region Prestamos

    public async Task<IEnumerable<RespuestaPrestamoDto>> ObtenerPrestamos(Guid userId, Guid userItemId,
        CancellationToken cancellationToken = default)
    {
        // Validar ownership del título antes de exponer su historial
        _ = await ObtenerUserItem(userId, userItemId, cancellationToken);

        List<Prestamo> prestamos = (await _unitOfWork.Prestamos.FindAsync(
            p => p.UserItemId == userItemId,
            cancellationToken: cancellationToken))
            .OrderByDescending(p => p.FechaPrestamo)
            .ToList();

        return prestamos.MapToDto();
    }

    public async Task<RespuestaPrestamoDto> AgregarPrestamo(Guid userId, Guid userItemId,
        PeticionCrearPrestamoDto dto, CancellationToken cancellationToken = default)
    {
        UserItem userItem = await ObtenerUserItem(userId, userItemId, cancellationToken);

        bool prestamoActivo = userItem.Prestamos?.Any(p => p.FechaDevolucion == null) ?? false;
        if (prestamoActivo)
        {
            throw new BusinessRuleException("Este título ya está prestado. Registra la devolución antes de prestarlo de nuevo.");
        }

        Prestamo prestamo = new Prestamo
        {
            UserItemId = userItem.Id,
            NombrePersona = dto.NombrePersona.Trim(),
            Notas = dto.Notas?.Trim(),
            FechaPrestamo = dto.FechaPrestamo ?? DateTimeOffset.UtcNow,
        };

        await _unitOfWork.Prestamos.AgregarAsync(prestamo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Préstamo {PrestamoId}: título {TituloId} de usuario {UserId} a {Persona}",
            prestamo.Id, userItemId, userId, prestamo.NombrePersona);

        return prestamo.MapToDto();
    }

    public async Task ActualizarPrestamo(Guid userId, Guid prestamoId, PeticionActualizarPrestamoDto dto,
        CancellationToken cancellationToken = default)
    {
        Prestamo prestamo = await ObtenerPrestamoPropio(userId, prestamoId, cancellationToken, track: true);

        if (dto.NombrePersona is not null) prestamo.NombrePersona = dto.NombrePersona.Trim();
        if (dto.Notas is not null) prestamo.Notas = dto.Notas.Trim();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Préstamo {PrestamoId} actualizado por el usuario {UserId}", prestamoId, userId);
    }

    public async Task RegistrarDevolucion(Guid userId, Guid prestamoId, DateTimeOffset? fechaDevolucion = null,
        CancellationToken cancellationToken = default)
    {
        Prestamo prestamo = await ObtenerPrestamoPropio(userId, prestamoId, cancellationToken, track: true);

        if (prestamo.FechaDevolucion is not null)
        {
            throw new BusinessRuleException("Este préstamo ya fue devuelto.");
        }

        DateTimeOffset fecha = fechaDevolucion ?? DateTimeOffset.UtcNow;
        if (fecha < prestamo.FechaPrestamo)
        {
            throw new BusinessRuleException("La fecha de devolución no puede ser anterior al préstamo.");
        }

        prestamo.FechaDevolucion = fecha;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Devolución registrada en el préstamo {PrestamoId} por el usuario {UserId}",
            prestamoId, userId);
    }

    public async Task EliminarPrestamo(Guid userId, Guid prestamoId, CancellationToken cancellationToken = default)
    {
        Prestamo prestamo = await ObtenerPrestamoPropio(userId, prestamoId, cancellationToken);
        _unitOfWork.Prestamos.Eliminar(prestamo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogWarning("Préstamo {PrestamoId} eliminado por el usuario {UserId}", prestamoId, userId);
    }

    /// <summary>Obtiene un préstamo validando que pertenezca a la biblioteca del usuario autenticado.</summary>
    private async Task<Prestamo> ObtenerPrestamoPropio(Guid userId, Guid prestamoId,
        CancellationToken cancellationToken, bool track = false)
    {
        Prestamo? prestamo = await _unitOfWork.Prestamos.GetFirstOrDefaultAsync(
            predicate: p => p.Id == prestamoId,
            includeProperties: "UserItem",
            cancellationToken: cancellationToken,
            disableTracking: !track);

        if (prestamo?.UserItem is null || prestamo.UserItem.UserId != userId)
        {
            _logger.LogWarning("El usuario {UserId} intentó acceder al préstamo {PrestamoId} de otro usuario",
                userId, prestamoId);
            throw new NotFoundException($"No se encontró el préstamo {prestamoId}");
        }

        return prestamo;
    }

    #endregion

    #region MetodosPrivados

    /// <summary>
    /// Valida que los formatos/plataformas de la copia propia existan en el catálogo.
    /// </summary>
    private async Task ValidarCopiaPropiaAsync(List<Guid> ownedFormatIds, List<Guid> ownedPlatformIds,
        CancellationToken cancellationToken)
    {
        if (ownedFormatIds.Count > 0)
        {
            HashSet<Guid> existentes = (await _unitOfWork.Formatos.FindAsync(
                f => ownedFormatIds.Contains(f.Id), cancellationToken)).Select(f => f.Id).ToHashSet();

            Guid faltante = ownedFormatIds.FirstOrDefault(id => !existentes.Contains(id));
            if (faltante != Guid.Empty)
            {
                throw new NotFoundException($"El formato {faltante} no existe.");
            }
        }

        if (ownedPlatformIds.Count > 0)
        {
            HashSet<Guid> existentes = (await _unitOfWork.Plataformas.FindAsync(
                p => ownedPlatformIds.Contains(p.Id), cancellationToken)).Select(p => p.Id).ToHashSet();

            Guid faltante = ownedPlatformIds.FirstOrDefault(id => !existentes.Contains(id));
            if (faltante != Guid.Empty)
            {
                throw new NotFoundException($"La plataforma {faltante} no existe.");
            }
        }
    }

    /// <summary>
    /// Sincroniza formatos/plataformas propios del UserItem con lo enviado por el DTO.
    /// null = no tocar esa lista; lista vacía = quitar todos.
    /// </summary>
    private async Task SincronizarCopiaPropiaAsync(UserItem userItem, PeticionActualizarUserItemDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.OwnedFormatIds is not null)
        {
            List<Guid> deseados = dto.OwnedFormatIds.Distinct().ToList();
            await ValidarCopiaPropiaAsync(deseados, new List<Guid>(), cancellationToken);

            ICollection<UserItemFormat> actuales = userItem.UserItemFormats!;
            foreach (UserItemFormat formato in actuales.Where(f => !deseados.Contains(f.FormatId)).ToList())
            {
                actuales.Remove(formato);
            }

            // Altas vía repositorio: evita que EF confunda filas nuevas con existentes
            HashSet<Guid> idsActuales = actuales.Select(f => f.FormatId).ToHashSet();
            foreach (Guid formatId in deseados.Where(id => !idsActuales.Contains(id)))
            {
                await _unitOfWork.ItemsUsuarioFormatos.AgregarAsync(
                    new UserItemFormat { FormatId = formatId, UserItemId = userItem.Id }, cancellationToken);
            }
        }

        if (dto.OwnedPlatformIds is not null)
        {
            List<Guid> deseados = dto.OwnedPlatformIds.Distinct().ToList();
            await ValidarCopiaPropiaAsync(new List<Guid>(), deseados, cancellationToken);

            ICollection<UserItemPlatform> actuales = userItem.UserItemPlatforms!;
            foreach (UserItemPlatform plataforma in actuales.Where(p => !deseados.Contains(p.PlatformId)).ToList())
            {
                actuales.Remove(plataforma);
            }

            HashSet<Guid> idsActuales = actuales.Select(p => p.PlatformId).ToHashSet();
            foreach (Guid platformId in deseados.Where(id => !idsActuales.Contains(id)))
            {
                await _unitOfWork.ItemsUsuarioPlataformas.AgregarAsync(
                    new UserItemPlatform { PlatformId = platformId, UserItemId = userItem.Id }, cancellationToken);
            }
        }
    }

    private async Task<UserItem> ObtenerUserItem(Guid userId, Guid userItemId, CancellationToken cancellationToken, bool track = false)
    {
        UserItem? userItem = await _unitOfWork.UserItems.GetFirstOrDefaultAsync(
            predicate: u => u.Id == userItemId,
            cancellationToken: cancellationToken,
            includeProperties: ItemIncludes.DesdeUserItem,
            disableTracking: !track);

        if (userItem is null)
        {
            _logger.LogWarning("No se encontró el UserItem {UserItemId}", userItemId);
            throw new NotFoundException($"No se encontró el elemento {userItemId}");
        }

        // Ownership: el recurso solo pertenece al usuario autenticado
        if (userItem.UserId != userId)
        {
            _logger.LogWarning("El usuario {UserId} intentó acceder al UserItem {UserItemId} de otro usuario", userId, userItemId);
            throw new NotFoundException($"No se encontró el elemento {userItemId}");
        }

        return userItem;
    }

    #endregion
}
using System.Linq.Expressions;
using BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.Exceptions;
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
            includeProperties: "Item.MediaType,Item.ItemFormats.Format,Item.ItemPlatforms.Platform,Item.ItemGenres.Genre,Item.ItemCreators.Creator,Item.ItemImages",
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

    public async Task<RespuestaBibliotecaStatsDto> ObtenerStats(Guid userId, CancellationToken cancellationToken = default)
    {
        List<UserItem> items = (await _unitOfWork.UserItems.FindAsync(
            u => u.UserId == userId, cancellationToken)).ToList();

        var ratings = items.Where(i => i.PersonalRating.HasValue).Select(i => (double)i.PersonalRating!.Value).ToList();

        RespuestaBibliotecaStatsDto stats = new RespuestaBibliotecaStatsDto
        {
            TotalItems = items.Count,
            Pendientes = items.Count(i => i.Status == ConsumptionStatus.Pendiente),
            EnProgreso = items.Count(i => i.Status == ConsumptionStatus.EnProgreso),
            Completados = items.Count(i => i.Status == ConsumptionStatus.Completado),
            Abandonados = items.Count(i => i.Status == ConsumptionStatus.Abandonado),
            Favoritos = items.Count(i => i.IsFavorite),
            RatingPromedio = ratings.Count == 0 ? 0 : Math.Round(ratings.Average() * 10) / 10
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

    #region MetodosPrivados

    private async Task<UserItem> ObtenerUserItem(Guid userId, Guid userItemId, CancellationToken cancellationToken, bool track = false)
    {
        UserItem? userItem = await _unitOfWork.UserItems.GetFirstOrDefaultAsync(
            predicate: u => u.Id == userItemId,
            cancellationToken: cancellationToken,
            includeProperties: "Item.MediaType,Item.ItemFormats.Format,Item.ItemPlatforms.Platform,Item.ItemGenres.Genre,Item.ItemCreators.Creator,Item.ItemImages",
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
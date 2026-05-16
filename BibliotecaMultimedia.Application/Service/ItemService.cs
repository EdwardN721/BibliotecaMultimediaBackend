using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Service;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IUnitOfWork unitOfWork, ILogger<ItemService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RespuestaPaginada<RespuestaItemDto>> ObtenerItemsPaginado(FiltroItem filtroItem, CancellationToken cancellationToken = default)
    {
        Expression<Func<Item, bool>>? filtro = null;

        if (!string.IsNullOrWhiteSpace(filtroItem.TerminoBusqueda))
        {
            string terminoBusqueda = filtroItem.TerminoBusqueda.ToLower();
            filtro = i => i.Title.ToLower().Contains(terminoBusqueda.ToLower());
        }

        (IEnumerable<Item> registros, int total) = await _unitOfWork.Items.ObtenerPaginadosAsync(
            filter: filtro,
            pageNumber: filtroItem.PageNumber,
            pageSize: filtroItem.PageSize,
            cancellationToken: cancellationToken);
        
        int totalPaginas = (int)Math.Ceiling(total / (double)filtroItem.PageSize);
        
        RespuestaPaginada<RespuestaItemDto> respuesta = registros
            .MapToDto()
            .ToRespuestaPaginada(total, totalPaginas, filtroItem.PageNumber, filtroItem.PageSize);
        
        _logger.LogInformation("Items paginados: Página {Page} de {TotalPages} con {Count} registros", 
            respuesta.Metadata.PaginaActual, respuesta.Metadata.TotalPaginas, respuesta.Registros.Count());
        return respuesta;
    }

    public async Task<IEnumerable<RespuestaItemDto>> ObtenerItems(CancellationToken cancellationToken = default)
    {
        List<Item> items = (await _unitOfWork.Items.ObtenerTodosAsync(cancellationToken)).ToList();
        
        _logger.LogInformation("Items paginados: {Count}", items.Count);
        return items.MapToDto();
    }

    public async Task<RespuestaItemDto> ObtenerItemPorId(Guid id, CancellationToken cancellationToken = default)
    {
        Item item = await ObtenerItem(id, track: false, cancellationToken);
        
        return item.MapToDto();
    }

    public async Task<RespuestaItemDto> AgregarItem(PeticionCrearItemDto itemDto, Guid currentUserId ,CancellationToken cancellationToken = default)
    {
        Item nuevoItem = itemDto.MapToEntity(currentUserId);
        
        foreach (Guid genreId in itemDto.GenreIds)
        {
            nuevoItem.ItemGenres?.Add(new ItemGenre { GenreId = genreId, ItemId = Guid.Empty});
        }
        
        foreach (Guid creatorId in itemDto.CreatorIds)
        {
            nuevoItem.ItemCreators?.Add(new ItemCreator { CreatorId = creatorId, ItemId = Guid.Empty, RoleId = Guid.Empty});
        }
        
        await _unitOfWork.Items.AgregarAsync(nuevoItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item agregado: {Id} - {Nombre}", nuevoItem.Id, nuevoItem.Title);
        return nuevoItem.MapToDto();
    }

    public async Task ActualizarItem(Guid id, PeticionActualizarItemDto itemDto, CancellationToken cancellationToken = default)
    {
        Item item = await ObtenerItem(id, track: false, cancellationToken);
        
        item.UpadteEntity(itemDto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Item actualizado: {Id} - {Nombre}", item.Id, item.Title);
    }

    public async Task EliminarItem(Guid id, CancellationToken cancellationToken = default)
    {
        Item item = await ObtenerItem(id, track: false, cancellationToken);
        
        _unitOfWork.Items.Eliminar(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogWarning("Item eliminado: {Id}", item.Id);
    }
    
    #region MetodosPrivados

    private async Task<Item> ObtenerItem(Guid id, bool track = true, CancellationToken cancellationToken = default)
    {
        Item? item = await _unitOfWork.Items.GetFirstOrDefaultAsync(
            predicate: i => i.Id == id,
            cancellationToken: cancellationToken,
            includeProperties: "MediaType,Format,Platform,ItemGenres.Genre,ItemCreators.Creator",
            disableTracking: !track
        );
        if (item == null)
        {
            _logger.LogWarning("No se encontro el item por el Id {id}", id);
            throw new NotFoundException($"No se encontro el item por el Id {id}");
        }
        return item;
    }

    #endregion
}
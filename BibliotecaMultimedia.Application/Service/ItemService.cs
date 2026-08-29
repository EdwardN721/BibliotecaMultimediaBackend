using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Domain.Constants;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.Common;
using BibliotecaMultimedia.Application.Extensions;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Eventos;
using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;


namespace BibliotecaMultimedia.Application.Service;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ItemService> _logger;
    private readonly IServiceBus _serviceBus;

    public ItemService(IUnitOfWork unitOfWork, ILogger<ItemService> logger, IServiceBus serviceBus)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
    }

    public async Task<RespuestaPaginada<RespuestaItemDto>> ObtenerItemsPaginado(FiltroItem filtroItem, CancellationToken cancellationToken = default)
    {
        Expression<Func<Item, bool>>? filtro = null;

        if (!string.IsNullOrWhiteSpace(filtroItem.TerminoBusqueda))
        {
            string terminoBusqueda = filtroItem.TerminoBusqueda.ToLower();
            filtro = i => i.Title.ToLower().Contains(terminoBusqueda.ToLower());
        }

        if (filtroItem.MediaTypeId.HasValue && filtroItem.MediaTypeId.Value != Guid.Empty)
        {
            Expression<Func<Item, bool>> filtroTipoMedio = i => i.MediaTypeId == filtroItem.MediaTypeId.Value;
            filtro = filtro == null ? filtroTipoMedio : filtro.And(filtroTipoMedio);
        }

        if (filtroItem.PlatformId.HasValue && filtroItem.PlatformId.Value != Guid.Empty)
        {
            Guid plataformaId = filtroItem.PlatformId.Value;
            Expression<Func<Item, bool>> filtroPlataforma = i => i.ItemPlatforms != null && i.ItemPlatforms.Any(p => p.PlatformId == plataformaId);
            filtro = filtro == null ? filtroPlataforma : filtro.And(filtroPlataforma);
        }

        if (filtroItem.GenreId.HasValue && filtroItem.GenreId.Value != Guid.Empty)
        {
            Guid generoId = filtroItem.GenreId.Value;
            Expression<Func<Item, bool>> filtroGenero = i => i.ItemGenres != null && i.ItemGenres.Any(g => g.GenreId == generoId);
            filtro = filtro == null ? filtroGenero : filtro.And(filtroGenero);
        }

        (IEnumerable<ItemMapper.ProyeccionItemDto> registros, int total) = await _unitOfWork.Items.ObtenerPaginadosProyectadosAsync(
            selector: ItemMapper.ProyeccionLista(),
            filter: filtro,
            pageNumber: filtroItem.PageNumber,
            pageSize: filtroItem.PageSize,
            ordenarPor: filtroItem.OrdenarPor,
            ordenDescendente: filtroItem.OrdenDescendente,
            cancellationToken: cancellationToken);
        
        int totalPaginas = (int)Math.Ceiling(total / (double)filtroItem.PageSize);
        
        RespuestaPaginada<RespuestaItemDto> respuesta = registros
            .MapProyeccionToDto()
            .ToRespuestaPaginada(total, totalPaginas, filtroItem.PageNumber, filtroItem.PageSize);
        
        _logger.LogInformation("Items paginados: Página {Page} de {TotalPages} con {Count} registros", 
            respuesta.Metadata.PaginaActual, respuesta.Metadata.TotalPaginas, respuesta.Registros.Count());
        return respuesta;
    }

    public async Task<IEnumerable<RespuestaItemDto>> ObtenerItems(CancellationToken cancellationToken = default)
    {
        // Proyección en BD (evita materializar UserItems y el grafo completo por ítem)
        IEnumerable<ItemMapper.ProyeccionItemDto> proyecciones = await _unitOfWork.Items.ObtenerTodosProyectadosAsync(
            ItemMapper.ProyeccionLista(),
            ordenarPor: nameof(Item.Title),
            ordenDescendente: false,
            cancellationToken);

        IEnumerable<RespuestaItemDto> items = proyecciones.MapProyeccionToDto();

        _logger.LogInformation("Items obtenidos: {Count}", items.Count());
        return items;
    }

    public async Task<IEnumerable<RespuestaItemDto>> ObtenerDestacados(int cantidad, CancellationToken cancellationToken = default)
    {
        if (cantidad < 1) cantidad = 1;
        if (cantidad > 50) cantidad = 50;

        // Ordenación y Take en SQL: no se materializa el catálogo completo en memoria
        (IEnumerable<ItemMapper.ProyeccionItemDto> registros, _) = await _unitOfWork.Items.ObtenerPaginadosProyectadosAsync(
            selector: ItemMapper.ProyeccionLista(),
            pageNumber: 1,
            pageSize: cantidad,
            ordenarPor: nameof(Item.CreatedAt),
            ordenDescendente: true,
            cancellationToken: cancellationToken);

        IEnumerable<RespuestaItemDto> destacados = registros.MapProyeccionToDto();

        _logger.LogInformation("Items destacados obtenidos: {Count}", destacados.Count());
        return destacados;
    }

    public async Task<IEnumerable<RespuestaDistribucionItemDto>> ObtenerDistribucionPorTipoMedio(
        CancellationToken cancellationToken = default)
    {
        // GROUP BY en la base de datos: no traemos el catálogo completo para
        // agregarlo en memoria del cliente o del servidor
        List<(string Clave, int Cantidad)> conteos = await _unitOfWork.Items.ContarAgrupadoAsync(
            i => i.MediaType!.Name,
            filtro: null,
            cancellationToken);

        int total = conteos.Sum(c => c.Cantidad);

        List<RespuestaDistribucionItemDto> distribucion = conteos
            .OrderByDescending(c => c.Cantidad)
            .ThenBy(c => c.Clave)
            .Select(c => new RespuestaDistribucionItemDto
            {
                Nombre = string.IsNullOrEmpty(c.Clave) ? "Otros" : c.Clave,
                Cantidad = c.Cantidad,
                Porcentaje = total == 0 ? 0 : Math.Round(c.Cantidad * 100.0 / total, 1),
            })
            .ToList();

        _logger.LogInformation("Distribución por tipo de medio: {Grupos} grupos, {Total} items", distribucion.Count, total);
        return distribucion;
    }

    public async Task<RespuestaItemDto> ObtenerItemPorId(Guid id, CancellationToken cancellationToken = default)
    {
        Item item = await ObtenerItem(id, track: false, cancellationToken);
        
        return item.MapToDto();
    }

    public async Task<RespuestaItemDto> AgregarItem(PeticionCrearItemDto itemDto, Guid currentUserId ,CancellationToken cancellationToken = default)
    {
        await ValidarReferenciasAsync(itemDto, cancellationToken);

        // Solo necesitamos el rol por defecto si el item trae creadores
        Guid roleId = Guid.Empty;
        if (itemDto.CreatorIds.Count > 0)
        {
            Role? rolPorDefecto = await _unitOfWork.CreatorRoles.GetFirstOrDefaultAsync(
                r => r.Name == RoleConstants.Author, cancellationToken, disableTracking: true)
                ?? throw new BusinessRuleException("No existe el rol de creador 'Autor' en el catálogo. Ejecute el sembrado de datos.");
            roleId = rolPorDefecto.Id;
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            Item nuevoItem = itemDto.MapToEntity(currentUserId);

            foreach (Guid genreId in itemDto.GenreIds)
            {
                nuevoItem.ItemGenres?.Add(new ItemGenre { GenreId = genreId, ItemId = Guid.Empty });
            }

            foreach (Guid creatorId in itemDto.CreatorIds)
            {
                nuevoItem.ItemCreators?.Add(new ItemCreator { CreatorId = creatorId, ItemId = Guid.Empty, RoleId = roleId });
            }

            foreach (Guid formatId in itemDto.FormatIds)
            {
                nuevoItem.ItemFormats?.Add(new ItemFormat { FormatId = formatId, ItemId = Guid.Empty });
            }

            foreach (Guid platformId in itemDto.PlatformIds)
            {
                nuevoItem.ItemPlatforms?.Add(new ItemPlatform { PlatformId = platformId, ItemId = Guid.Empty });
            }

            await _unitOfWork.Items.AgregarAsync(nuevoItem, cancellationToken);

            // CommitTransactionAsync ya ejecuta SaveChanges internamente (evita un save redundante).
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Item agregado: {Id} - {Nombre}", nuevoItem.Id, nuevoItem.Title);

            // Publicamos DESPUÉS del commit: si el guardado falla no se notifica nada,
            // y si Azure falla el item ya está persistido (solo registramos el error).
            try
            {
                ItemAgregadoEvento evento = nuevoItem.ToDto(currentUserId);
                await _serviceBus.NotificarAgregacionAsync(evento, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "El item {Id} se guardó pero no se pudo notificar al Service Bus", nuevoItem.Id);
            }

            return nuevoItem.MapToDto();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task ActualizarItem(Guid id, PeticionActualizarItemDto itemDto, CancellationToken cancellationToken = default)    {
        await ValidarReferenciasActualizarAsync(itemDto, cancellationToken);
        Item item = await ObtenerItem(id, track: true, cancellationToken);

        Guid defaultRoleId = Guid.Empty;
        if (itemDto.CreatorIds.Count > 0)
        {
            Role? rolPorDefecto = await _unitOfWork.CreatorRoles.GetFirstOrDefaultAsync(
                r => r.Name == RoleConstants.Author, cancellationToken, disableTracking: true)
                ?? throw new BusinessRuleException("No existe el rol de creador 'Autor' en el catálogo. Ejecute el sembrado de datos.");
            defaultRoleId = rolPorDefecto.Id;
        }

        item.UpdateEntity(itemDto);
        await SincronizarRelacionesAsync(item, itemDto, defaultRoleId, cancellationToken);
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

    private Task ValidarReferenciasAsync(PeticionCrearItemDto itemDto, CancellationToken cancellationToken)
        => ValidarReferenciasComunesAsync(itemDto.MediaTypeId, itemDto.FormatIds, itemDto.PlatformIds,
            itemDto.GenreIds, itemDto.CreatorIds, cancellationToken);

    private Task ValidarReferenciasActualizarAsync(PeticionActualizarItemDto itemDto, CancellationToken cancellationToken)
        => ValidarReferenciasComunesAsync(itemDto.MediaTypeId, itemDto.FormatIds, itemDto.PlatformIds,
            itemDto.GenreIds, itemDto.CreatorIds, cancellationToken);

    private async Task ValidarReferenciasComunesAsync(
        Guid mediaTypeId,
        List<Guid> formatIds,
        List<Guid> platformIds,
        List<Guid> genreIds,
        List<Guid> creatorIds,
        CancellationToken cancellationToken)
    {
        bool mediaTypeExiste = await _unitOfWork.TiposMedia.GetFirstOrDefaultAsync(
            m => m.Id == mediaTypeId, cancellationToken, disableTracking: true) is not null;
        if (!mediaTypeExiste)
        {
            throw new NotFoundException($"El tipo de medio {mediaTypeId} no existe.");
        }

        if (formatIds.Count > 0)
        {
            HashSet<Guid> formatosExistentes = (await _unitOfWork.Formatos.FindAsync(
                f => formatIds.Contains(f.Id), cancellationToken))
                .Select(f => f.Id).ToHashSet();

            Guid formatoFaltante = formatIds.FirstOrDefault(id => !formatosExistentes.Contains(id));
            if (formatoFaltante != Guid.Empty)
            {
                throw new NotFoundException($"El formato {formatoFaltante} no existe.");
            }
        }
        if (platformIds.Count > 0)
        {
            HashSet<Guid> plataformasExistentes = (await _unitOfWork.Plataformas.FindAsync(
                p => platformIds.Contains(p.Id), cancellationToken))
                .Select(p => p.Id).ToHashSet();

            Guid plataformaFaltante = platformIds.FirstOrDefault(id => !plataformasExistentes.Contains(id));
            if (plataformaFaltante != Guid.Empty)
            {
                throw new NotFoundException($"La plataforma {plataformaFaltante} no existe.");
            }
        }

        if (genreIds.Count > 0)
        {
            HashSet<Guid> generosExistentes = (await _unitOfWork.Generos.FindAsync(
                g => genreIds.Contains(g.Id), cancellationToken))
                .Select(g => g.Id).ToHashSet();

            Guid generoFaltante = genreIds.FirstOrDefault(id => !generosExistentes.Contains(id));
            if (generoFaltante != Guid.Empty)
            {
                throw new NotFoundException($"El género {generoFaltante} no existe.");
            }
        }

        if (creatorIds.Count > 0)
        {
            HashSet<Guid> creadoresExistentes = (await _unitOfWork.Creadores.FindAsync(
                c => creatorIds.Contains(c.Id), cancellationToken))
                .Select(c => c.Id).ToHashSet();

            Guid creadorFaltante = creatorIds.FirstOrDefault(id => !creadoresExistentes.Contains(id));
            if (creadorFaltante != Guid.Empty)
            {
                throw new NotFoundException($"El creador {creadorFaltante} no existe.");
            }
        }
    }

    private async Task SincronizarRelacionesAsync(Item item, PeticionActualizarItemDto itemDto, Guid defaultRoleId, CancellationToken cancellationToken)
    {
        ICollection<ItemGenre> generos = item.ItemGenres!;
        ICollection<ItemCreator> creadores = item.ItemCreators!;
        ICollection<ItemFormat> formatos = item.ItemFormats!;
        ICollection<ItemPlatform> plataformas = item.ItemPlatforms!;

        // Bajas: quitar de las colecciones tracked (el interceptor de auditoría las convierte en soft delete)
        foreach (ItemGenre genero in generos.Where(g => !itemDto.GenreIds.Contains(g.GenreId)).ToList())
        {
            generos.Remove(genero);
        }

        foreach (ItemCreator creador in creadores.Where(c => !itemDto.CreatorIds.Contains(c.CreatorId)).ToList())
        {
            creadores.Remove(creador);
        }

        foreach (ItemFormat formato in formatos.Where(f => !itemDto.FormatIds.Contains(f.FormatId)).ToList())
        {
            formatos.Remove(formato);
        }

        foreach (ItemPlatform plataforma in plataformas.Where(p => !itemDto.PlatformIds.Contains(p.PlatformId)).ToList())
        {
            plataformas.Remove(plataforma);
        }

        // Altas: registrar por repositorio (Add explícito). No basta con agregar a la
        // navegación: como BaseEntity inicializa Id = Guid.NewGuid(), EF creería que la
        // fila ya existe y emitiría un UPDATE contra una fila inexistente (error 409).
        HashSet<Guid> generosActuales = generos.Select(g => g.GenreId).ToHashSet();
        foreach (Guid genreId in itemDto.GenreIds.Where(id => !generosActuales.Contains(id)))
        {
            await _unitOfWork.ItemsGeneros.AgregarAsync(new ItemGenre { GenreId = genreId, ItemId = item.Id }, cancellationToken);
        }

        HashSet<Guid> creadoresActuales = creadores.Select(c => c.CreatorId).ToHashSet();
        foreach (Guid creatorId in itemDto.CreatorIds.Where(id => !creadoresActuales.Contains(id)))
        {
            await _unitOfWork.ItemsCreadores.AgregarAsync(new ItemCreator { CreatorId = creatorId, ItemId = item.Id, RoleId = defaultRoleId }, cancellationToken);
        }

        HashSet<Guid> formatosActuales = formatos.Select(f => f.FormatId).ToHashSet();
        foreach (Guid formatId in itemDto.FormatIds.Where(id => !formatosActuales.Contains(id)))
        {
            await _unitOfWork.ItemsFormatos.AgregarAsync(new ItemFormat { FormatId = formatId, ItemId = item.Id }, cancellationToken);
        }

        HashSet<Guid> plataformasActuales = plataformas.Select(p => p.PlatformId).ToHashSet();
        foreach (Guid platformId in itemDto.PlatformIds.Where(id => !plataformasActuales.Contains(id)))
        {
            await _unitOfWork.ItemsPlataformas.AgregarAsync(new ItemPlatform { PlatformId = platformId, ItemId = item.Id }, cancellationToken);
        }
    }

    private async Task<Item> ObtenerItem(Guid id, bool track = true, CancellationToken cancellationToken = default)
    {
        Item? item = await _unitOfWork.Items.GetFirstOrDefaultAsync(
            predicate: i => i.Id == id,
            cancellationToken: cancellationToken,
            includeProperties: ItemIncludes.DesdeItem,
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
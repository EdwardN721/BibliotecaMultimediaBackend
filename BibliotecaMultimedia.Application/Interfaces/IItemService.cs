using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Items;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Paginacion;
using BibliotecaMultimedia.Application.DTOs.Peticion.Paginacion.Filtros;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IItemService
{
    Task<RespuestaPaginada<RespuestaItemDto>> ObtenerItemsPaginado(FiltroItem filtroItem, CancellationToken cancellationToken = default);
    Task<IEnumerable<RespuestaItemDto>> ObtenerItems(CancellationToken cancellationToken = default);
    Task<IEnumerable<RespuestaItemDto>> ObtenerDestacados(int cantidad = 12, CancellationToken cancellationToken = default);
    Task<IEnumerable<RespuestaDistribucionItemDto>> ObtenerDistribucionPorTipoMedio(CancellationToken cancellationToken = default);
    Task<RespuestaItemDto> ObtenerItemPorId(Guid id, CancellationToken cancellationToken = default);
    Task<RespuestaItemDto> AgregarItem(PeticionCrearItemDto itemDto, Guid currentUserId,
        CancellationToken cancellationToken = default);
    Task ActualizarItem(Guid id, PeticionActualizarItemDto itemDto, CancellationToken cancellationToken = default);
    Task EliminarItem(Guid id, CancellationToken cancellationToken = default);
    
}
using BibliotecaMultimedia.Application.DTOs.Eventos;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class EventosMapper
{
    public static ItemAgregadoEvento ToDto(this Item item, Guid currentUserId)
    {
        return new ItemAgregadoEvento
        {
            ItemId = item.Id,
            Title = item.Title,
            UserId = currentUserId,
            CreadoEn = DateTimeOffset.UtcNow,
        };
    } 
}
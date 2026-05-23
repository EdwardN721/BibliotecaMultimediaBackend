using BibliotecaMultimedia.Application.DTOs.Eventos;

namespace BibliotecaMultimedia.Application.Interfaces;

public interface IServiceBus
{
    Task NotificarAgregacionAsync(ItemAgregadoEvento item, CancellationToken cancellationToken = default);
}
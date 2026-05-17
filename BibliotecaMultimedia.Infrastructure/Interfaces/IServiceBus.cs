namespace BibliotecaMultimedia.Infrastructure.Interfaces;

public interface IServiceBus
{
    Task NotificarAgregacionAsync(Guid itemId, string nombreTitulo, CancellationToken cancellationToken = default);
}
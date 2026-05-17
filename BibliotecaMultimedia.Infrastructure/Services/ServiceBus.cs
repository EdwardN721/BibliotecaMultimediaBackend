using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BibliotecaMultimedia.Infrastructure.Interfaces;

namespace BibliotecaMultimedia.Infrastructure.Services;

/// <summary>
/// Servicio de infraestructura que agregara a una cola para notificar al usuario sobre su
/// informacion
/// </summary>
public class ServiceBus : IServiceBus, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public ServiceBus(string connectionString, string topicName)
    {
        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(topicName);
    }

    /// <summary>
    /// Notificar al usuario
    /// </summary>
    /// <param name="itemId">Item que se agrego</param>
    /// <param name="nombreTitulo">Titulo que se agrego</param>
    /// <param name="cancellationToken">Token para cancelacion asincrona</param>
    public async Task NotificarAgregacionAsync(Guid itemId, string nombreTitulo, CancellationToken cancellationToken = default)
    {
        var mensaje = new
        {
            Evento = "AgregarNuevoArticulo",
            ItemId = itemId,
            Titulo = nombreTitulo,
            Fecha = DateTimeOffset.UtcNow,
        };
        
        string json = JsonSerializer.Serialize(mensaje);
        ServiceBusMessage serviceBusMessage = new ServiceBusMessage(json);
        
        await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
        await _sender.DisposeAsync();
    }
}
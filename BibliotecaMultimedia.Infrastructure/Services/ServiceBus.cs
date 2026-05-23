using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BibliotecaMultimedia.Application.DTOs.Eventos;
using BibliotecaMultimedia.Application.Interfaces;

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
    /// <param name="evento">Información del evento a notificar</param>
    /// <param name="cancellationToken">Token para cancelacion asincrona</param>
    public async Task NotificarAgregacionAsync(ItemAgregadoEvento evento, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(evento);
        ServiceBusMessage message = new ServiceBusMessage(json)
        {
            Subject = "ItemAgregado"
        };
        
        await _sender.SendMessageAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
        await _sender.DisposeAsync();
    }
}
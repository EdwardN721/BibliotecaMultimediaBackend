using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BibliotecaMultimedia.Application.DTOs.Eventos;
using BibliotecaMultimedia.Application.Interfaces;

namespace BibliotecaMultimedia.Infrastructure.Services;

/// <summary>
/// Publicador de eventos hacia una cola de Azure Service Bus.
/// El cliente y el sender se crean una sola vez: son thread-safe y su
/// reutilización es la práctica recomendada por Microsoft.
/// </summary>
public class ServiceBus : IServiceBus, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private const string EventTypeItemAgregado = "ItemAgregado";

    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public ServiceBus(string connectionString, string queueName)
    {
        // El SDK reintenta automáticamente (3 intentos, backoff exponencial).
        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(queueName);
    }

    /// <summary>
    /// Publica el evento de ítem agregado en la cola.
    /// </summary>
    /// <param name="evento">Información del evento a notificar</param>
    /// <param name="cancellationToken">Token para cancelación asíncrona</param>
    public async Task NotificarAgregacionAsync(ItemAgregadoEvento evento, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(evento, JsonOptions);

        ServiceBusMessage message = new ServiceBusMessage(json)
        {
            // Id determinista: permite deduplicación e idempotencia en el consumidor
            MessageId = evento.ItemId.ToString(),

            // Metadatos estándar para consumidores
            ContentType = "application/json",

            // El tipo de evento viaja como propiedad de aplicación,
            // que es la forma recomendada para filtrar/enrutar mensajes
            ApplicationProperties = { ["eventType"] = EventTypeItemAgregado },
        };

        await _sender.SendMessageAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        // Al disponer el cliente se liberan también los senders/receivers hijos
        await _client.DisposeAsync();
    }
}

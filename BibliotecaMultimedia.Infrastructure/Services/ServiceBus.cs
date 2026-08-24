using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BibliotecaMultimedia.Application.DTOs.Eventos;
using BibliotecaMultimedia.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BibliotecaMultimedia.Infrastructure.Services;

/// <summary>
/// Publicador de eventos hacia una cola de Azure Service Bus.
/// El cliente y el sender se crean una sola vez: son thread-safe y su
/// reutilización es la práctica recomendada por Microsoft.
///
/// Degradación elegante: si la cadena de conexión es inválida o placeholder
/// (típico en un clone limpio sin secretos), el servicio queda DESHABILITADO
/// y solo registra warnings en lugar de romper los flujos que lo resuelven.
/// </summary>
public class ServiceBus : IServiceBus, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private const string EventTypeItemAgregado = "ItemAgregado";

    private readonly ILogger<ServiceBus> _logger;
    private readonly ServiceBusClient? _client;
    private readonly ServiceBusSender? _sender;
    private readonly string _queueName;

    public ServiceBus(string connectionString, string queueName, ILogger<ServiceBus> logger)
    {
        _logger = logger;
        _queueName = queueName;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogWarning("AzureServiceBus sin cadena de conexión: el publicador de eventos queda deshabilitado.");
            return;
        }

        try
        {
            // El SDK reintenta automáticamente (3 intentos, backoff exponencial).
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(queueName);
        }
        catch (FormatException ex)
        {
            // Cadena placeholder/malformada: no tiramos la dependencia completa,
            // solo dejamos el publicador inactivo para no bloquear desarrollo local.
            _logger.LogWarning(
                "AzureServiceBus con cadena de conexión inválida ({Motivo}): el publicador de eventos queda deshabilitado.",
                ex.Message);
            _client = null;
            _sender = null;
        }
    }

    /// <summary>
    /// Indica si el publicador está listo para enviar mensajes.
    /// </summary>
    public bool Habilitado => _sender is not null;

    /// <summary>
    /// Publica el evento de ítem agregado en la cola.
    /// </summary>
    /// <param name="evento">Información del evento a notificar</param>
    /// <param name="cancellationToken">Token para cancelación asíncrona</param>
    public async Task NotificarAgregacionAsync(ItemAgregadoEvento evento, CancellationToken cancellationToken = default)
    {
        if (_sender is null)
        {
            _logger.LogWarning(
                "Evento ItemAgregado del item {ItemId} NO publicado: Service Bus deshabilitado (cola '{Cola}').",
                evento.ItemId, _queueName);
            return;
        }

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
        if (_client is not null)
        {
            await _client.DisposeAsync();
        }
    }
}

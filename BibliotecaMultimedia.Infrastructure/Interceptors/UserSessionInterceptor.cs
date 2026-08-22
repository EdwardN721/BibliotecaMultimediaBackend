using System.Data.Common;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BibliotecaMultimedia.Infrastructure.Interceptors;

public class UserSessionInterceptor : DbConnectionInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserSessionInterceptor> _logger;

    public UserSessionInterceptor(IHttpContextAccessor httpContextAccessor, ILogger<UserSessionInterceptor> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        await EstablecerUsuarioAsync(connection, cancellationToken);
    }

    public override async Task ConnectionClosedAsync(DbConnection connection, ConnectionEndEventData eventData)
    {
        // Con connection pooling la conexión se reutiliza entre usuarios:
        // limpiamos la variable de sesión para no filtrar identidad entre requests.
        await LimpiarUsuarioAsync(connection);
        await base.ConnectionClosedAsync(connection, eventData);
    }

    #region MetodosPrivados

    private async Task EstablecerUsuarioAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        Guid? userId = ObtenerUserIdDelContexto();
        if (userId is null)
        {
            return; // Operaciones sin usuario (seeder, arranque, migraciones)
        }

        try
        {
            using DbCommand comando = connection.CreateCommand();
            // userId es un Guid parseado: seguro de interpolar
            comando.CommandText = $"SELECT set_config('app.current_user_id', '{userId}', false);";
            await comando.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // No interrumpimos la operación por un fallo de sesión; solo registramos
            _logger.LogError(ex, "No se pudo establecer app.current_user_id para el usuario {UserId}", userId);
        }
    }

    private async Task LimpiarUsuarioAsync(DbConnection connection)
    {
        try
        {
            using DbCommand comando = connection.CreateCommand();
            comando.CommandText = "RESET app.current_user_id;";
            await comando.ExecuteNonQueryAsync(CancellationToken.None);
        }
        catch (ObjectDisposedException)
        {
            // La conexión ya fue liberada; nada que limpiar
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo limpiar app.current_user_id al cerrar la conexión");
        }
    }

    private Guid? ObtenerUserIdDelContexto()
    {
        string? userIdClaim = _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
    }

    #endregion
}

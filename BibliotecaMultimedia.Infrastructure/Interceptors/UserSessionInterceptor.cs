using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BibliotecaMultimedia.Infrastructure.Interceptors;

public class UserSessionInterceptor : DbConnectionInterceptor
{
    // En un caso real, inyectarías IHttpContextAccessor aquí para sacar el ID del token JWT
    private readonly string _currentUserId = "00000000-0000-0000-0000-000000000001"; // TODO: Reemplazar con el usuario real

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        await SetCurrentUserSessionAsync(connection, cancellationToken);
    }

    private async Task SetCurrentUserSessionAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        using var command = connection.CreateCommand();
        // Inyectamos el ID en la sesión local de PostgreSQL para que el Trigger lo lea
        command.CommandText = $"SET LOCAL app.current_user_id = '{_currentUserId}';";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
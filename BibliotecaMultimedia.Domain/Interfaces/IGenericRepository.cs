using System.Linq.Expressions;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> ObtenerTodosAsync(string? includeProperties = null, CancellationToken cancellationToken = default);
    Task<T?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties);

    // Soporta sub-niveles (ThenInclude) usando strings
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        string? includeProperties = null,
        bool disableTracking = false);

    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties);

    Task<(IEnumerable<T> Registros, int Total)> ObtenerPaginadosAsync(
        Expression<Func<T, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default,
        string? includeProperties = null,
        string? ordenarPor = null,
        bool ordenDescendente = false,
        params Expression<Func<T, object>>[] includes);

    Task<(IEnumerable<TResult> Registros, int Total)> ObtenerPaginadosProyectadosAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default,
        string? ordenarPor = null,
        bool ordenDescendente = false);

    /// <summary>
    /// Cuenta registros agrupados por una clave, traducido a GROUP BY en la BD
    /// (evita traer todos los registros para agregarlos en memoria).
    /// </summary>
    Task<List<(TKey Clave, int Cantidad)>> ContarAgrupadoAsync<TKey>(
        Expression<Func<T, TKey>> agruparPor,
        Expression<Func<T, bool>>? filtro = null,
        CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AgregarAsync(T entity, CancellationToken cancellationToken = default);
    void Actualizar(T entity);
    void Eliminar(T entity);
}
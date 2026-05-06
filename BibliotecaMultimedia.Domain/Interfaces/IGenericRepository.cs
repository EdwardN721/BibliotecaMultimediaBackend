using System.Linq.Expressions;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<T?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties);

    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties);

    Task<(IEnumerable<T> Registros, int Total)> ObtenerPaginadosAsync(
        Expression<Func<T, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task AgregarAsync(T entity, CancellationToken cancellationToken = default);
    void Actualizar(T entity);
    void Eliminar(T entity);
}
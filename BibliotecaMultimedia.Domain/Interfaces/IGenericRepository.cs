using System.Linq.Expressions;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> ObtenerTodosAsync();
    Task<T?> ObtenerPorIdAsync(Guid id);

    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties);

    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties);

    Task<(IEnumerable<T> Registros, int Total)> ObtenerPaginadosAsync(
        Expression<Func<T, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 10,
        params Expression<Func<T, object>>[] includes);

    Task AgregarAsync(T entity);
    void Actualizar(T entity);
    void Eliminar(T entity);
}
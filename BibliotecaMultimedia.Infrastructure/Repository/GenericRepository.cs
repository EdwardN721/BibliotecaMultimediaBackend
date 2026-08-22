using System.Linq.Expressions;
using System.Reflection;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMultimedia.Infrastructure.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<T>> ObtenerTodosAsync(string? includeProperties = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(includeProperties))
        {
            query = IncluirPropiedades(includeProperties, query);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet.AsNoTracking().Where(filter);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<T> Registros, int Total)> ObtenerPaginadosAsync(
        Expression<Func<T, bool>>? filtro = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default,
        string? includeProperties = null,
        string? ordenarPor = null,
        bool ordenDescendente = false,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (filtro != null)
        {
            query = query.Where(filtro);
        }

        foreach (var includeProperty in includes)
        {
            query = query.Include(includeProperty);
        }

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            query = IncluirPropiedades(includeProperties, query);
        }

        int total = await query.CountAsync(cancellationToken);

        bool tieneIncludes = includes.Length > 0 || !string.IsNullOrWhiteSpace(includeProperties);
        if (tieneIncludes)
        {
            query = query.AsSplitQuery();
        }

        query = AplicarOrden(query, ordenarPor, ordenDescendente);

        var registros = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (registros, total);
    }

    public async Task<(IEnumerable<TResult> Registros, int Total)> ObtenerPaginadosProyectadosAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? filtro = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default,
        string? ordenarPor = null,
        bool ordenDescendente = false)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (filtro != null)
        {
            query = query.Where(filtro);
        }

        int total = await query.CountAsync(cancellationToken);

        query = AplicarOrden(query, ordenarPor, ordenDescendente);

        var registros = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return (registros, total);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    // Soporta sub-niveles (ThenInclude) usando strings
    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        string? includeProperties = null,
        bool disableTracking = false)
    {
        IQueryable<T> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            query = IncluirPropiedades(includeProperties, query);
        }

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task AgregarAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Actualizar(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Eliminar(T entity)
    {
        if (_dbSet.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    #region MetodosPrivados

    private IQueryable<T> IncluirPropiedades(string propiedades, IQueryable<T> query)
    {
        foreach (var propiedad in propiedades.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(propiedad.Trim());
        }

        return query;
    }

    private static IQueryable<T> AplicarOrden(IQueryable<T> query, string? ordenarPor, bool ordenDescendente)
    {
        // Solo ordenamos por propiedades públicas existentes en la entidad:
        // evita un ArgumentException (500) si el cliente envía un nombre inválido.
        PropertyInfo? propiedad = typeof(T).GetProperty(
            ordenarPor?.Trim() ?? string.Empty,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (!string.IsNullOrWhiteSpace(ordenarPor) && propiedad is not null)
        {
            var parametro = Expression.Parameter(typeof(T), "x");
            var cuerpo = Expression.Property(parametro, propiedad);
            var selector = Expression.Lambda(cuerpo, parametro);

            string metodo = ordenDescendente ? "OrderByDescending" : "OrderBy";
            System.Reflection.MethodInfo? metodoOrden = typeof(Queryable).GetMethods()
                .First(m => m.Name == metodo && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), propiedad.PropertyType);

            return (IQueryable<T>)metodoOrden.Invoke(null, new object[] { query, selector })!;
        }

        return ordenDescendente ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
    }

    #endregion
}
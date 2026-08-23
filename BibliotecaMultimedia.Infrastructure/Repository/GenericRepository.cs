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

    public async Task<List<(TKey Clave, int Cantidad)>> ContarAgrupadoAsync<TKey>(
        Expression<Func<T, TKey>> agruparPor,
        Expression<Func<T, bool>>? filtro = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (filtro != null)
        {
            query = query.Where(filtro);
        }

        var filas = await query
            .GroupBy(agruparPor)
            .Select(g => new { Clave = g.Key, Cantidad = g.Count() })
            .ToListAsync(cancellationToken);

        return filas.Select(r => (r.Clave, r.Cantidad)).ToList();
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

    // Lista blanca de propiedades por las que el cliente puede pedir orden:
    // evita un 500 al intentar traducir a SQL columnas no ordenables
    // (p. ej. Metadata jsonb) o nombres arbitrarios.
    private static readonly Dictionary<Type, HashSet<string>> PropiedadesOrdenPermitidas = new()
    {
        [typeof(Item)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(Item.Title), nameof(BaseEntity.CreatedAt), nameof(Item.ReleaseDate), nameof(BaseEntity.UpdatedAt) },
        [typeof(UserItem)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(BaseEntity.CreatedAt), nameof(UserItem.DateAdded), nameof(UserItem.StartedAt), nameof(UserItem.FinishedAt), nameof(UserItem.PersonalRating) },
        [typeof(Creator)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(Creator.Name), nameof(BaseEntity.CreatedAt) },
        [typeof(Genre)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(Genre.Name), nameof(BaseEntity.CreatedAt) },
        [typeof(Format)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(Format.Name), nameof(BaseEntity.CreatedAt) },
        [typeof(MediaType)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(MediaType.Name), nameof(BaseEntity.CreatedAt) },
        [typeof(Platform)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(Platform.Name), nameof(BaseEntity.CreatedAt) },
        [typeof(Role)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(Role.Name), nameof(BaseEntity.CreatedAt) },
        [typeof(ItemImage)] = new(StringComparer.OrdinalIgnoreCase)
            { nameof(BaseEntity.CreatedAt), nameof(ItemImage.IsPrimary) },
    };

    private static IQueryable<T> AplicarOrden(IQueryable<T> query, string? ordenarPor, bool ordenDescendente)
    {
        if (!string.IsNullOrWhiteSpace(ordenarPor)
            && PropiedadesOrdenPermitidas.TryGetValue(typeof(T), out HashSet<string>? permitidas))
        {
            PropertyInfo? propiedad = typeof(T).GetProperty(
                ordenarPor.Trim(),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (propiedad is not null && permitidas.Contains(propiedad.Name))
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
        }

        return ordenDescendente ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
    }

    #endregion
}
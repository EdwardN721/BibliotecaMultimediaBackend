using System.Linq.Expressions;
using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMultimedia.Infrastructure.Repositoy;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    
    public async Task<T?> ObtenerPorIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> ObtenerTodosAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet.AsNoTracking().Where(filter);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<T> Registros, int Total)> ObtenerPaginadosAsync(
        Expression<Func<T, bool>>? filtro = null,
        int pageNumber = 1,
        int pageSize = 10,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (filtro != null)
        {
            query = query.Where(filtro);
        }
        
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        int total = await query.CountAsync();

        var registros = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (registros, total);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet;
        
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task AgregarAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
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
}
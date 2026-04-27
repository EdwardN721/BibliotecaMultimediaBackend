using BibliotecaMultimedia.Domain.Interfaces;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace BibliotecaMultimedia.Infrastructure.Repositoy;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    private IGenericRepository<Creator>? _creadores;
    private IGenericRepository<Format>? _formatos;
    private IGenericRepository<Genre>? _generos;
    private IGenericRepository<Item>? _items;
    private IGenericRepository<ItemCreator>? _creadoresItems;
    private IGenericRepository<ItemGenre>? _creadoresGeneros;
    private IGenericRepository<ItemImage>? _creadoresImagenes;
    private IGenericRepository<MediaType>? _tiposMedia;
    private IGenericRepository<Platform>? _plataformas;
    private IGenericRepository<Role>? _roles;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Creator> Creadores
    {
        get { return _creadores ??= new GenericRepository<Creator>(_context); }
    }

    public IGenericRepository<Format> Formatos
    {
        get { return _formatos ??= new GenericRepository<Format>(_context); }
    }

    public IGenericRepository<Genre> Generos
    {
        get { return _generos ??= new GenericRepository<Genre>(_context); }
    }

    public IGenericRepository<Item> Items
    {
        get { return _items ??= new GenericRepository<Item>(_context); }
    }

    public IGenericRepository<ItemCreator> CreadoresItems
    {
        get { return _creadoresItems ??= new GenericRepository<ItemCreator>(_context); }
    }

    public IGenericRepository<ItemGenre> CreadoresGeneros
    {
        get { return _creadoresGeneros ??= new GenericRepository<ItemGenre>(_context); }
    }

    public IGenericRepository<ItemImage> CreadoresImagenes
    {
        get { return _creadoresImagenes ??= new GenericRepository<ItemImage>(_context); }
    }

    public IGenericRepository<MediaType> TiposMedia
    {
        get { return _tiposMedia ??= new GenericRepository<MediaType>(_context); }
    }

    public IGenericRepository<Platform> Plataformas
    {
        get { return _plataformas ??= new GenericRepository<Platform>(_context); }
    }

    public IGenericRepository<Role> Roles
    {
        get { return _roles ??= new GenericRepository<Role>(_context); }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            return;
        }
        _transaction = await _context.Database.BeginTransactionAsync(); 
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;    
            }
        }
    }
    
    public void Dispose()
    {
        if (_transaction != null)
        {
            _transaction.Dispose();
        }
        _context.Dispose();
    }
}
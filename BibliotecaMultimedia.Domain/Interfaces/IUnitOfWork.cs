using System.Linq.Expressions;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Creator> Creadores { get; }
    IGenericRepository<Format> Formatos { get; }
    IGenericRepository<Genre> Generos { get; }
    IGenericRepository<Item> Items { get; }
    IGenericRepository<ItemCreator> CreadoresItems { get; }
    IGenericRepository<ItemGenre> CreadoresGeneros { get; }
    IGenericRepository<ItemImage> CreadoresImagenes { get; }
    IGenericRepository<MediaType> TiposMedia { get; }
    IGenericRepository<Platform> Plataformas { get; }
    IGenericRepository<Role> Roles { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    // Gestión de Transacciones especificas
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
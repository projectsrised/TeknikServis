using System.Linq.Expressions;
using TeknikServisApp.Domain.Entities;

namespace TeknikServisApp.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> Query();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

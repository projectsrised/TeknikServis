using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;
using TeknikServisApp.Infrastructure.Data;

namespace TeknikServisApp.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.Silindi = true;
        _dbSet.Update(entity);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(_context);
        }
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

public class TenantProvider : ITenantProvider, ICurrentUserService
{
    public Guid TenantId { get; set; }
    public Guid? BayiId { get; set; }
    public Guid? KullaniciId { get; set; }
    public TeknikServisApp.Domain.Enums.UserRole Rol { get; set; }

    // ICurrentUserService implementation
    public Guid? UserId => KullaniciId;
    public bool IsSuperAdmin => Rol == TeknikServisApp.Domain.Enums.UserRole.SuperAdmin;
}

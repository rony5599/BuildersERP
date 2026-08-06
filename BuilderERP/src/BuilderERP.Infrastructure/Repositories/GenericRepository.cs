using System.Linq.Expressions;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

    public async Task<IReadOnlyList<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public IQueryable<T> Query() => _dbSet.AsQueryable();

    public IQueryable<T> Find(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate);

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);
}

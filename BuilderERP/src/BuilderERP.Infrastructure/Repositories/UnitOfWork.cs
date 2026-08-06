using BuilderERP.Domain.Interfaces;
using BuilderERP.Infrastructure.Persistence;

namespace BuilderERP.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new GenericRepository<T>(_context);
            _repositories[type] = repository;
        }

        return (IRepository<T>)repository;
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

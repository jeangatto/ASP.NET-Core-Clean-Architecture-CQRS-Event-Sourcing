using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Core.Abstractions;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories.Common;

internal abstract class BaseWriteOnlyRepository<TEntity> : IWriteOnlyRepository<TEntity>
    where TEntity : class, IEntity<Guid>
{
    private readonly DbSet<TEntity> _dbSet;
    protected readonly WriteDbContext Context;

    protected BaseWriteOnlyRepository(WriteDbContext context)
    {
        Context = context;
        _dbSet = context.Set<TEntity>();
    }

    public void Add(TEntity entity) =>
        _dbSet.Add(entity);

    public void Update(TEntity entity) =>
        _dbSet.Update(entity);

    public void Remove(TEntity entity) =>
        _dbSet.Remove(entity);

    public async Task<TEntity> GetByIdAsync(Guid id) =>
        await _dbSet.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(entity => entity.Id == id);

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~BaseWriteOnlyRepository()
        => Dispose(false);

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // Dispose managed state (managed objects).
        if (disposing)
            Context.Dispose();

        _disposed = true;
    }

    #endregion
}
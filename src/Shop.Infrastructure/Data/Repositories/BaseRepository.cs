using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Core.Abstractions;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

public abstract class BaseRepository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly DbSet<TEntity> DbSet;
    private readonly ShopContext _context;

    protected BaseRepository(ShopContext context)
    {
        _context = context;
        DbSet = _context.Set<TEntity>();
    }

    public void Add(TEntity entity)
        => DbSet.Add(entity);

    public void AddRange(IEnumerable<TEntity> entities)
        => DbSet.AddRange(entities);

    public void Update(TEntity entity)
        => DbSet.Update(entity);

    public void UpdateRange(IEnumerable<TEntity> entities)
        => DbSet.UpdateRange(entities);

    public void Remove(TEntity entity)
        => DbSet.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities)
        => DbSet.RemoveRange(entities);

    public async Task<TEntity> GetByIdAsync(
        Guid id,
        bool readOnly = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = readOnly ? DbSet.AsNoTracking() : DbSet.AsQueryable();
        return await queryable.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.ToListAsync(cancellationToken);

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~BaseRepository()
    {
        Dispose(false);
    }

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
            _context.Dispose();

        _disposed = true;
    }

    #endregion
}
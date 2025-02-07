using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Core.SharedKernel;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories.Common;

/// <summary>
/// Base class for write-only repositories.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
internal abstract class BaseWriteOnlyRepository<TEntity, TKey>(WriteDbContext context) : IWriteOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private static readonly Func<WriteDbContext, TKey, Task<TEntity>> GetByIdCompiledAsync =
        EF.CompileAsyncQuery((WriteDbContext context, TKey id) =>
            context
                .Set<TEntity>()
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(entity => entity.Id.Equals(id)));

    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    protected readonly WriteDbContext Context = context;

    public void Add(TEntity entity) =>
        _dbSet.Add(entity);

    public void Update(TEntity entity) =>
        _dbSet.Update(entity);

    public void Remove(TEntity entity) =>
        _dbSet.Remove(entity);

    public async Task<TEntity> GetByIdAsync(TKey id) =>
        await GetByIdCompiledAsync(Context, id);

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    ~BaseWriteOnlyRepository() => Dispose(false);

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
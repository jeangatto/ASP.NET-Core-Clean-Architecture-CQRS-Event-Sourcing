using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Core.Abstractions;

/// <summary>
/// Repositório (somente escrita).
/// </summary>
/// <typeparam name="TEntity">O tipo da entidade.</typeparam>
public interface IWriteOnlyRepository<TEntity> : IDisposable
    where TEntity : IEntity<Guid>
{
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Obtém a entidade por Id.
    /// Utilizado somente para efetuar UPDATE ou DELETE.
    /// </summary>
    /// <param name="id">O Id.</param>
    /// <returns>A entidade se existir, caso contrário; nulo.</returns>
    Task<TEntity> GetByIdAsync(Guid id);
}
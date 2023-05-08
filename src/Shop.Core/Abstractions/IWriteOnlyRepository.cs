using System;
using System.Threading.Tasks;

namespace Shop.Core.Abstractions;

/// <summary>
/// Repositório (somente escrita).
/// </summary>
/// <typeparam name="TEntity">O tipo da entidade.</typeparam>
public interface IWriteOnlyRepository<TEntity> : IDisposable where TEntity : IEntity<Guid>
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);

    /// <summary>
    /// Obtém a entidade por Id.
    /// Utilizado somente para efetuar UPDATE ou DELETE.
    /// </summary>
    /// <param name="id">O Id.</param>
    /// <returns>A entidade se existir, caso contrário; nulo.</returns>
    Task<TEntity> GetByIdAsync(Guid id);
}
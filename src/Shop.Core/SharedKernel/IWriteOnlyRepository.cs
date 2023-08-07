using System;
using System.Threading.Tasks;

namespace Shop.Core.SharedKernel;

/// <summary>
/// Represents a repository that allows write-only operations on entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public interface IWriteOnlyRepository<TEntity> : IDisposable where TEntity : IEntity<Guid>
{
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Retrieves an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>The retrieved entity.</returns>
    Task<TEntity> GetByIdAsync(Guid id);
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

/// <summary>
/// Repositório (somente escrita).
/// </summary>
/// <typeparam name="T">O tipo da entidade.</typeparam>
/// <typeparam name="TKey">O tipo da chave.</typeparam>
public interface IWriteOnlyRepository<T, in TKey> where T : IEntity<TKey> where TKey : IEquatable<TKey>
{
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Obtém a entidade por Id.
    /// </summary>
    /// <param name="id">O Id.</param>
    /// <returns>A entidade se existir, caso contrário; nulo.</returns>
    Task<T> GetByIdAsync(TKey id);
}
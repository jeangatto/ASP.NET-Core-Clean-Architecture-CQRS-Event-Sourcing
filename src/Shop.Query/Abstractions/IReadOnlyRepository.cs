using System;
using System.Threading.Tasks;

namespace Shop.Query.Abstractions;

/// <summary>
/// Repositório (somente leitura).
/// </summary>
/// <typeparam name="T">O tipo do modelo da query.</typeparam>
/// <typeparam name="TKey">O tipo de chave.</typeparam>
public interface IReadOnlyRepository<T, in TKey>
    where T : IQueryModel<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Obtém o modelo da query por Id.
    /// </summary>
    /// <param name="id">O Id.</param>
    /// <returns>O modelo se existir, caso contrário; nulo.</returns>
    Task<T> GetByIdAsync(TKey id);
}
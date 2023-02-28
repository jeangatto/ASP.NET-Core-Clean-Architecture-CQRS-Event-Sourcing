using System;
using System.Threading.Tasks;

namespace Shop.Query.Abstractions;

/// <summary>
/// Repositório (somente leitura).
/// </summary>
/// <typeparam name="TQueryModel">O tipo do modelo da query.</typeparam>
public interface IReadOnlyRepository<TQueryModel> : IDisposable
    where TQueryModel : IQueryModel<Guid>
{
    /// <summary>
    /// Obtém o modelo da query por Id.
    /// </summary>
    /// <param name="id">O Id.</param>
    /// <returns>O modelo se existir, caso contrário; nulo.</returns>
    Task<TQueryModel> GetByIdAsync(Guid id);
}
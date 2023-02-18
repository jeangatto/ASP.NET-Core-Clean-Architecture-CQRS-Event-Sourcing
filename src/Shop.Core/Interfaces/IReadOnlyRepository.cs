using System;
using System.Threading.Tasks;
using Shop.Core.Abstractions;

namespace Shop.Core.Interfaces;

/// <summary>
/// Repositório (somente leitura).
/// </summary>
/// <typeparam name="TQueryModel">O tipo do modelo.</typeparam>
public interface IReadOnlyRepository<TQueryModel> where TQueryModel : BaseQueryModel
{
    /// <summary>
    /// Obtém o modelo por Id.
    /// </summary>
    /// <param name="id">O Id.</param>
    /// <returns>O modelo se existir, caso contrário; nulo.</returns>
    Task<TQueryModel> GetByIdAsync(Guid id);
}
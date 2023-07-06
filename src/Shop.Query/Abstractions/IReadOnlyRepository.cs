using System;
using System.Threading.Tasks;

namespace Shop.Query.Abstractions;

/// <summary>
/// Represents a read-only repository interface.
/// </summary>
/// <typeparam name="TQueryModel">The type of the query model.</typeparam>
public interface IReadOnlyRepository<TQueryModel> where TQueryModel : IQueryModel<Guid>
{
    /// <summary>
    /// Gets the query model by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the query model.</param>
    /// <returns>The task representing the asynchronous operation, returning the query model.</returns>
    Task<TQueryModel> GetByIdAsync(Guid id);
}
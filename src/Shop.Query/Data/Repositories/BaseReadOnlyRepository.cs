using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Query.Abstractions;

namespace Shop.Query.Data.Repositories;

/// <summary>
/// Base repository class for read-only operations.
/// </summary>
/// <typeparam name="TQueryModel">The type of the query model.</typeparam>
/// <typeparam name="Tkey">The type of the key.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseReadOnlyRepository{TQueryModel, Tkey}"/> class.
/// </remarks>
/// <param name="context">The read database context.</param>
internal abstract class BaseReadOnlyRepository<TQueryModel, Tkey>(IReadDbContext context) : IReadOnlyRepository<TQueryModel, Tkey>
    where TQueryModel : IQueryModel<Tkey>
    where Tkey : IEquatable<Tkey>
{
    protected readonly IMongoCollection<TQueryModel> Collection = context.GetCollection<TQueryModel>();

    /// <summary>
    /// Gets a query model by its id.
    /// </summary>
    /// <param name="id">The id of the query model.</param>
    /// <returns>The query model.</returns>
    public async Task<TQueryModel> GetByIdAsync(Tkey id) =>
        await Collection.Find(queryModel => queryModel.Id.Equals(id)).FirstOrDefaultAsync();
}
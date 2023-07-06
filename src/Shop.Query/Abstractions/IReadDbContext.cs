using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Shop.Query.Abstractions;

public interface IReadDbContext
{
    /// <summary>
    /// Gets the connection string for the database.
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the collection for the specified query model.
    /// </summary>
    /// <typeparam name="TQueryModel">The type of the query model.</typeparam>
    /// <returns>The MongoDB collection for the specified query model.</returns>
    IMongoCollection<TQueryModel> GetCollection<TQueryModel>()
        where TQueryModel : IQueryModel;

    /// <summary>
    /// Upserts a query model into the database.
    /// </summary>
    /// <typeparam name="TQueryModel">The type of the query model.</typeparam>
    /// <param name="queryModel">The query model to upsert.</param>
    /// <param name="upsertFilter">The filter expression to determine the upsert condition.</param>
    /// <returns>A task representing the asynchronous upsert operation.</returns>
    Task UpsertAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel;

    /// <summary>
    /// Deletes query models from the database that match the specified filter.
    /// </summary>
    /// <typeparam name="TQueryModel">The type of the query model.</typeparam>
    /// <param name="deleteFilter">The filter expression to determine which query models to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAsync<TQueryModel>(Expression<Func<TQueryModel, bool>> deleteFilter)
        where TQueryModel : IQueryModel;

    /// <summary>
    /// Creates collections in the database for all query models.
    /// </summary>
    /// <returns>A task representing the asynchronous creation of collections.</returns>
    Task CreateCollectionsAsync();
}
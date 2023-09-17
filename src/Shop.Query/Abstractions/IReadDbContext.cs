using System.Threading.Tasks;
using MongoDB.Driver;

namespace Shop.Query.Abstractions;

/// <summary>
/// Represents the read-only database context for querying data.
/// </summary>
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
    IMongoCollection<TQueryModel> GetCollection<TQueryModel>() where TQueryModel : IQueryModel;

    /// <summary>
    /// Creates collections in the database for all query models.
    /// </summary>
    /// <returns>A task representing the asynchronous creation of collections.</returns>
    Task CreateCollectionsAsync();
}

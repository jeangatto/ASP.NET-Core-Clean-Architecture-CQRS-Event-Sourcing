using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shop.Core.AppSettings;

namespace Shop.Infrastructure.Data.Context;

public class ReadDbContext
{
    private const string DatabaseName = "Events";

    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;

    public ReadDbContext(IOptions<ConnectionOptions> options)
    {
        _client = new MongoClient(options.Value.EventConnection);
        _database = _client.GetDatabase(DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>() => _database.GetCollection<T>(typeof(T).Name);
}
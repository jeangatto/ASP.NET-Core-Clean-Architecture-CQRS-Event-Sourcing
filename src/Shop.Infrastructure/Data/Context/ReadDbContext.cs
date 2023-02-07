using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shop.Core.AppSettings;
using Shop.Core.Events;
using Shop.Domain.Entities.Customer;
using Shop.Domain.QueriesModel;

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

    public IMongoCollection<CustomerQueryModel> Customers => _database.GetCollection<CustomerQueryModel>(nameof(Customer));
    public IMongoCollection<EventStore> EventStores => _database.GetCollection<EventStore>(nameof(EventStore));
}
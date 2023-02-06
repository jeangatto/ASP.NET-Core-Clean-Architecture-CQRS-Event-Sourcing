using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shop.Core.AppSettings;
using Shop.Core.Events;

namespace Shop.Infrastructure.Data.Events;

public class EventContext
{
    private const string DatabaseName = "Events";
    private readonly IMongoDatabase _mongoDatabase;

    public EventContext(IOptions<ConnectionOptions> options)
    {
        var mongoClient = new MongoClient(options.Value.EventConnection);
        _mongoDatabase = mongoClient.GetDatabase(DatabaseName);
    }

    public IMongoCollection<StoredEvent> StoredEvents => _mongoDatabase.GetCollection<StoredEvent>(nameof(StoredEvent));
}
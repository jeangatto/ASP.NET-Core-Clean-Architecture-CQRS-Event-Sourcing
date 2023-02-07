using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Core.Events;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly ReadDbContext _readDbContext;

    public EventStoreRepository(ReadDbContext readDbContext)
        => _readDbContext = readDbContext;

    public async Task InsertManyAsync(IEnumerable<EventStore> eventStores)
    {
        var collection = _readDbContext.GetCollection<EventStore>();
        await collection.InsertManyAsync(eventStores);
    }
}
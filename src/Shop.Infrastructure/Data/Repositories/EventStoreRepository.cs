using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Core.Events;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly EventStoreDbContext _context;

    public EventStoreRepository(EventStoreDbContext context) => _context = context;

    public async Task StoreAsync(IEnumerable<EventStore> eventStores)
    {
        await _context.EventStores.AddRangeAsync(eventStores);
        await _context.SaveChangesAsync();
    }
}
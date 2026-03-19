using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Core.SharedKernel;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

internal sealed class EventStoreRepository(EventStoreDbContext dbContext) : IEventStoreRepository
{
    public async Task StoreAsync(IEnumerable<EventStore> eventStores)
    {
        await dbContext.EventStores.AddRangeAsync(eventStores);
        await dbContext.SaveChangesAsync();
    }
}
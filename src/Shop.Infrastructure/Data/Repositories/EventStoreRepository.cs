using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Core.SharedKernel;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

internal sealed class EventStoreRepository : IEventStoreRepository
{
    private readonly EventStoreDbContext _context;

    public EventStoreRepository(EventStoreDbContext context) =>
        _context = context;

    public async Task StoreAsync(IEnumerable<EventStore> eventStores)
    {
        await _context.EventStores.AddRangeAsync(eventStores);
        await _context.SaveChangesAsync();
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~EventStoreRepository()
        => Dispose(false);

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // Dispose managed state (managed objects).
        if (disposing)
            _context.Dispose();

        _disposed = true;
    }

    #endregion
}
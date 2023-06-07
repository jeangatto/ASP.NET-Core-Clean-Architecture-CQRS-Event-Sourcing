using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Core.SharedKernel;

/// <summary>
/// Repositório de armazenamento de eventos.
/// </summary>
public interface IEventStoreRepository : IDisposable
{
    /// <summary>
    /// Salva uma lista de eventos.
    /// </summary>
    /// <param name="eventStores">A lista de eventos.</param>
    Task StoreAsync(IEnumerable<EventStore> eventStores);
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Core.Events;

/// <summary>
/// Repositório de armazenamento de eventos.
/// </summary>
public interface IEventStoreRepository
{
    /// <summary>
    /// Salva uma lista de eventos.
    /// </summary>
    /// <param name="eventStores">A lista de eventos.</param>
    Task InsertManyAsync(IEnumerable<EventStore> eventStores);
}
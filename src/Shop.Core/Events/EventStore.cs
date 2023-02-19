using System;
using Shop.Core.Abstractions;

namespace Shop.Core.Events;

/// <summary>
/// A classe de armazenamento de evento.
/// </summary>
public class EventStore : BaseDomainEvent
{
    public EventStore(string type, string data)
    {
        Type = type;
        Data = data;
    }

    private EventStore() { }

    /// <summary>
    /// ID do evento.
    /// </summary>
    public Guid Id { get; private init; } = Guid.NewGuid();

    /// <summary>
    /// O tipo do evento.
    /// </summary>
    public string Type { get; private init; }

    /// <summary>
    /// O dadso do evento serializado em JSON.
    /// </summary>
    public string Data { get; private init; }
}
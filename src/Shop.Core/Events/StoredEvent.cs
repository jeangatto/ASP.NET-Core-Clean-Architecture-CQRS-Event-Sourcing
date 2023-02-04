using System;
using Shop.Core.Abstractions;

namespace Shop.Core.Events;

public class StoredEvent : BaseDomainEvent
{
    public StoredEvent(string type, string data)
    {
        Type = type;
        Data = data;
    }

    private StoredEvent() { }

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
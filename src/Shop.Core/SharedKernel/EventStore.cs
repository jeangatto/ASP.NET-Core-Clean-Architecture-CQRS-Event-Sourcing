using System;

namespace Shop.Core.SharedKernel;

/// <summary>
/// A classe de armazenamento de evento.
/// </summary>
public class EventStore : BaseEvent
{
    public EventStore(Guid aggregateId, string messageType, string data)
    {
        AggregateId = aggregateId;
        MessageType = messageType;
        Data = data;
    }

    public EventStore() { } // Only for EF/ORM

    /// <summary>
    /// ID do evento.
    /// </summary>
    public Guid Id { get; private init; } = Guid.NewGuid();

    /// <summary>
    /// Os dados do evento serializado em JSON.
    /// </summary>
    public string Data { get; private init; }
}
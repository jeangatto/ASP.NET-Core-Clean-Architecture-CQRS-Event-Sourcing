using System;

namespace Shop.Core.SharedKernel;

public class EventStore : BaseEvent
{
    public EventStore(Guid aggregateId, string messageType, string data)
    {
        AggregateId = aggregateId;
        MessageType = messageType;
        Data = data;
    }

    public EventStore() { } // Default constructor for EF/ORM

    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Data { get; private init; }
}
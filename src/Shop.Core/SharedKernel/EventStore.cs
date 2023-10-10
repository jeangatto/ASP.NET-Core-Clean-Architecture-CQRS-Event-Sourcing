using System;

namespace Shop.Core.SharedKernel;

/// <summary>
/// Represents the event store for storing events.
/// </summary>
public class EventStore : BaseEvent
{
    /// <summary>
    /// Initializes a new instance of the EventStore class.
    /// </summary>
    /// <param name="aggregateId">The aggregate ID.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="data">The data.</param>
    public EventStore(Guid aggregateId, string messageType, string data)
    {
        AggregateId = aggregateId;
        MessageType = messageType;
        Data = data;
    }

    /// <summary>
    /// Default constructor for Entity Framework or other ORM frameworks.
    /// </summary>
    public EventStore()
    {
    }

    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    public Guid Id { get; private init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public string Data { get; private init; }
}
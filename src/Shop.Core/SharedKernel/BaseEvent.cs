using System;
using MediatR;

namespace Shop.Core.SharedKernel;

/// <summary>
/// Evento.
/// </summary>
public abstract class BaseEvent : INotification
{
    /// <summary>
    /// O tipo do evento.
    /// </summary>
    public string MessageType { get; protected init; }

    /// <summary>
    /// ID da entidade.
    /// </summary>
    public Guid AggregateId { get; protected init; }

    /// <summary>
    /// Data e hora de quando ocorreu o evento.
    /// </summary>
    public DateTime OccurredOn { get; private init; } = DateTime.Now;
}
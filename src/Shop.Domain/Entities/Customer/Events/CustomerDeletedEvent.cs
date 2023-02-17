using System;
using Shop.Core.Abstractions;

namespace Shop.Domain.Entities.Customer.Events;

/// <summary>
/// Evento que representa um cliente deletado.
/// </summary>
public class CustomerDeletedEvent : BaseDomainEvent
{
    public CustomerDeletedEvent(Guid id) => Id = id;

    public Guid Id { get; private init; }
}
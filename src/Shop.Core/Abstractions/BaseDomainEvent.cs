using System;
using Shop.Core.Interfaces;

namespace Shop.Core.Abstractions;

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; private init; } = DateTime.Now;
}
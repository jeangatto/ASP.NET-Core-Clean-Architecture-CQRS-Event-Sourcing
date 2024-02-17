using System;
using Shop.Core.SharedKernel;

namespace Shop.Domain.Entities.ProductAggregate.Events;

public abstract class ProductBaseEvent : BaseEvent
{
    protected ProductBaseEvent(
        Guid id,
        string name,
        string description,
        decimal price)
    {
        Id = id;
        AggregateId = id;
        Name = name;
        Description = description;
        Price = price;
    }

    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string Description { get; private init; }
    public decimal Price { get; private init; }
}
using System;

namespace Shop.Domain.Entities.ProductAggregate.Events;

public class ProductUpdatedEvent(
    Guid id,
    string name,
    string description,
    decimal price) : ProductBaseEvent(id, name, description, price)
{
}
using System;

namespace Shop.Domain.Entities.ProductAggregate.Events;

public class ProductDeletedEvent(
    Guid id,
    string name,
    string description,
    decimal price) : ProductBaseEvent(id, name, description, price)
{
}
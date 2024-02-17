using Shop.Domain.Entities.ProductAggregate;

namespace Shop.Domain.Factories;

public static class ProductFactory
{
    public static Product Create(string name, string description, decimal price)
        => new(name, description, price);
}
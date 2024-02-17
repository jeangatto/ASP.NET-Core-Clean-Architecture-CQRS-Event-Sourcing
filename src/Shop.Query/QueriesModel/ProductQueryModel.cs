using System;
using Shop.Query.Abstractions;

namespace Shop.Query.QueriesModel;

public class ProductQueryModel : IQueryModel<Guid>
{
    public ProductQueryModel(
        Guid id,
        string name,
        string description,
        decimal price)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
    }

    private ProductQueryModel()
    {
    }

    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string Description { get; private init; }
    public decimal Price { get; private init; }
}

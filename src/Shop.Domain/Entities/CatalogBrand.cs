using Shop.Core;
using Shop.Core.Interfaces;

namespace Shop.Domain.Entities;

public class CatalogBrand : BaseEntity, IAggregateRoot
{
    public CatalogBrand(string brand)
    {
        Brand = brand;
    }

    private CatalogBrand() { } // EF Core

    public string Brand { get; private set; }
}
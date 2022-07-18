using System;
using Shop.Core;
using Shop.Core.Interfaces;

namespace Shop.Domain.Entities;

public class CatalogItem : BaseEntity, IAggregateRoot
{
    public CatalogItem(
        Guid catalogTypeId,
        Guid catalogBrandId,
        string description,
        string name,
        decimal price,
        string pictureUri)
    {
        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
        Description = description;
        Name = name;
        Price = price;
        PictureUri = pictureUri;
    }

    private CatalogItem() { } // EF Core

    public Guid CatalogTypeId { get; private set; }
    public Guid CatalogBrandId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string PictureUri { get; private set; }

    public CatalogType CatalogType { get; private set; }
    public CatalogBrand CatalogBrand { get; private set; }
}
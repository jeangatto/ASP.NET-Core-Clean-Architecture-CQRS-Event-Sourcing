using Shop.Core.SharedKernel;
using Shop.Domain.Entities.ProductAggregate.Events;

namespace Shop.Domain.Entities.ProductAggregate;

public class Product : BaseEntity, IAggregateRoot
{
    private bool _isDeleted;

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets or sets the description of the product.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Product class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="price"></param>
    public Product(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;

        AddDomainEvent(new ProductCreatedEvent(Id, name, description, price));
    }

    /// <summary>
    /// Default constructor for Entity Framework or other ORM frameworks.
    /// </summary>
    public Product()
    {
    }

    /// <summary>
    /// Changes the name of the Product.
    /// </summary>
    /// <param name="newName"></param>
    public void ChangeName(string newName)
    {
        if (Name.Equals(newName))
            return;

        Name = newName;
        AddDomainEvent(new ProductUpdatedEvent(Id, newName, Description, Price));
    }

    /// <summary>
    /// Changes the description of the Product.
    /// </summary>
    /// <param name="newDescription"></param>
    public void ChangeDescription(string newDescription)
    {
        if (Description.Equals(newDescription))
            return;

        Description = newDescription;
        AddDomainEvent(new ProductUpdatedEvent(Id, Name, newDescription, Price));
    }

    public void ChangePrice(decimal newPrice)
    {
        if (Price.Equals(newPrice))
            return;

        Price = newPrice;
        AddDomainEvent(new ProductUpdatedEvent(Id, Name, Description, newPrice));
    }

    /// <summary>
    /// Deletes the Product.
    /// </summary>
    public void Delete()
    {
        if (_isDeleted) return;

        _isDeleted = true;
        AddDomainEvent(new ProductDeletedEvent(Id, Name, Description, Price));
    }
}

using Bogus;
using FluentAssertions;
using Shop.Domain.Entities.ProductAggregate;
using Shop.Domain.Entities.ProductAggregate.Events;
using Shop.Domain.Factories;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Domain.Entities.ProductAggregate;

[UnitTest]
public class ProductTests
{
    [Fact]
    public void Should_ProductCreatedEvent_WhenCreate()
    {
        // Arrange
        var productFaker = new Faker<Product>()
            .CustomInstantiator(faker => ProductFactory.Create(
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                decimal.Parse(faker.Commerce.Price())));

        // Act
        var act = productFaker.Generate();

        // Assert
        act.DomainEvents.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.ContainItemsAssignableTo<ProductCreatedEvent>();
    }

    [Fact]
    public void Should_ProductUpdatedEvent_WhenChangeName()
    {
        // Arrange
        var productEntity = new Faker<Product>()
            .CustomInstantiator(faker => ProductFactory.Create(
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                decimal.Parse(faker.Commerce.Price())))
            .Generate();

        var newName = new Faker<string>()
            .CustomInstantiator(faker => faker.Commerce.ProductName())
            .Generate();

        // Act
        productEntity.ChangeName(newName);

        // Assert
        productEntity.DomainEvents.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.ContainItemsAssignableTo<ProductUpdatedEvent>();
    }

    [Fact]
    public void Should_ProductUpdatedEvent_WhenChangeDescription()
    {
        // Arrange
        var productEntity = new Faker<Product>()
            .CustomInstantiator(faker => ProductFactory.Create(
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                decimal.Parse(faker.Commerce.Price())))
            .Generate();

        var newDescription = new Faker<string>()
            .CustomInstantiator(faker => faker.Lorem.Sentence())
            .Generate();

        // Act
        productEntity.ChangeDescription(newDescription);

        // Assert
        productEntity.DomainEvents.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.ContainItemsAssignableTo<ProductUpdatedEvent>();
    }

    [Fact]
    public void Should_ProductUpdatedEvent_WhenChangePrice()
    {
        // Arrange
        var productEntity = new Faker<Product>()
            .CustomInstantiator(faker => ProductFactory.Create(
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                decimal.Parse(faker.Commerce.Price())))
            .Generate();

        var newPrice = 10;

        // Act
        productEntity.ChangePrice(newPrice);

        // Assert
        productEntity.DomainEvents.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.ContainItemsAssignableTo<ProductUpdatedEvent>();
    }

    [Fact]
    public void Should_ProductDeletedEvent_WhenDelete()
    {
        // Arrange
        var productEntity = new Faker<Product>()
            .CustomInstantiator(faker => ProductFactory.Create(
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                decimal.Parse(faker.Commerce.Price())))
            .Generate();

        // Act
        productEntity.Delete();

        // Assert
        productEntity.DomainEvents.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.ContainItemsAssignableTo<ProductDeletedEvent>();
    }
}

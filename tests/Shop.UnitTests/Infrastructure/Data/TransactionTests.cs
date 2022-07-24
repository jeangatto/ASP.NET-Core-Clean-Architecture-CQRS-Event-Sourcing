using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.Context;
using Shop.UnitTests.Fixtures;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Infrastructure.Data;

[UnitTest]
public class TransactionTests : IClassFixture<EfSqliteFixture>
{
    private readonly EfSqliteFixture _fixture;

    public TransactionTests(EfSqliteFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_NotThrowException_WhenExecuteAsync()
    {
        // Arrange
        var catalogBrands = GetPreconfiguredCatalogBrands();
        var catalogTypes = GetPreconfiguredCatalogTypes();
        var transaction = new Transaction<ShopContext>(_fixture.Context, Mock.Of<ILogger<ShopContext>>());

        // Act
        Func<Task> act = async () =>
        {
            await transaction.ExecuteAsync(() =>
            {
                _fixture.Context.CatalogBrands.AddRange(catalogBrands);
                _fixture.Context.CatalogTypes.AddRange(catalogTypes);
                return Task.CompletedTask;
            });
        };

        // Assert
        await act.Should().NotThrowAsync();
        _fixture.Context.CatalogBrands.Count().Should().Be(catalogBrands.Count());
        _fixture.Context.CatalogTypes.Count().Should().Be(catalogTypes.Count());
    }

    private static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands() => new List<CatalogBrand>
    {
        new("Azure"),
        new(".NET"),
        new("Visual Studio"),
        new("SQL Server"),
        new("Other")
    };

    private static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes() => new List<CatalogType>
    {
        new("Mug"),
        new("T-Shirt"),
        new("Sheet"),
        new("USB Memory Stick")
    };
}
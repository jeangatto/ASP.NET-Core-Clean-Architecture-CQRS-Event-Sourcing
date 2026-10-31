using FluentAssertions;
using Shop.Core.Extensions;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Core.Extensions;

[UnitTest]
public class AssemblyExtensionsTests
{
    [Fact]
    public void GetAllTypesOf_ShouldReturnTypesImplementingInterface()
    {
        // Arrange
        var assembly = typeof(AssemblyExtensionsTests).Assembly;

        // Act
        var types = assembly.GetAllTypesOf<object>();

        // Assert
        types.Should().NotBeNull();
        types.Should().Contain(typeof(AssemblyExtensionsTests));
    }

    [Fact]
    public void GetAllTypesOf_ShouldReturnDomainEntitiesImplementingInterface()
    {
        // Arrange
        var assembly = typeof(Customer).Assembly;

        // Act
        var types = assembly.GetAllTypesOf<IEntity>();

        // Assert
        types.Should().Contain(typeof(Customer))
            .And.NotContain(type => type.IsInterface || type.IsAbstract);
    }
}